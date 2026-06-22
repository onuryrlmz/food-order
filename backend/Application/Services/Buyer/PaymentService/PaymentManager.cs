using Application.Services.Seller.CommissionService;
using Base.Enums;
using Domain.Entities.Buyer;
using Domain.Entities.Common;
using Domain.Entities.Seller;
using Domain.Service;
using Application.Services.Common.NotificationService;
using Hangfire;
using Infrastructure.Adapters.IyzicoServiceAdapter;
using Infrastructure.Adapters.OneSignalAdapter;
using Microsoft.EntityFrameworkCore;
using Persistence.Contexts;
using Persistence.IRepositories;

namespace Application.Services.Buyer.PaymentService;

public class PaymentManager : IPaymentService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IIyzicoServiceAdapter _iyzicoAdapter;
    private readonly BaseDbContext _context;
    private readonly INotificationService _notificationService;
    private readonly IRealtimeNotifier _realtimeNotifier;
    private readonly ICommissionService _commissionService;

    public PaymentManager(
        IUnitOfWork unitOfWork,
        IIyzicoServiceAdapter iyzicoAdapter,
        BaseDbContext context,
        INotificationService notificationService,
        IRealtimeNotifier realtimeNotifier,
        ICommissionService commissionService)
    {
        _unitOfWork = unitOfWork;
        _iyzicoAdapter = iyzicoAdapter;
        _context = context;
        _notificationService = notificationService;
        _realtimeNotifier = realtimeNotifier;
        _commissionService = commissionService;
    }

    public async Task<Payment> CreatePendingPayment(Guid orderId, Guid userId, Guid sellerId, decimal amount, int paymentOptionId, string? providerConversationId, string? cardAlias = null)
    {
        var payment = new Payment
        {
            Id = Guid.NewGuid(),
            OrderId = orderId,
            UserId = userId,
            SellerId = sellerId,
            Amount = amount,
            SellerPayoutAmount = 0,
            CommissionAmount = 0,
            StatusId = (short)PaymentStatusEnums.Pending,
            PaymentOptionId = paymentOptionId,
            ProviderConversationId = providerConversationId,
            CardAlias = cardAlias
        };

        await _unitOfWork.PaymentRepository.AddAsync(payment);
        await _context.SaveChangesAsync();

        return payment;
    }

    public async Task<ServiceObjectResult<bool>> HandlePaymentCallback(string conversationId, string paymentId, string conversationData, bool isSuccess, string? errorMessage = null)
    {
        var result = new ServiceObjectResult<bool>();
        try
        {
            // conversationId = orderId (PlaceOrder'da böyle set ettik)
            if (!Guid.TryParse(conversationId, out var orderId))
            {
                result.Fail("Geçersiz conversationId.");
                return result;
            }

            var order = await _unitOfWork.OrderRepository.GetAsync(x => x.Id == orderId, enableTracking: true);
            if (order == null)
            {
                result.Fail("Sipariş bulunamadı.");
                return result;
            }

            var payment = await _unitOfWork.PaymentRepository.GetAsync(
                x => x.OrderId == orderId && x.StatusId == (short)PaymentStatusEnums.Pending,
                enableTracking: true);

            if (isSuccess)
            {
                // 3DS tamamla
                var completeResult = await _iyzicoAdapter.CompleteThreeDsPayment(conversationId, paymentId, conversationData);

                if (!completeResult.HasFailed && completeResult.Data?.Success == true)
                {
                    // Ödeme başarılı
                    if (payment != null)
                    {
                        payment.StatusId = (short)PaymentStatusEnums.Completed;
                        payment.ProviderPaymentId = paymentId;
                        payment.CompletedAt = DateTime.UtcNow;

                        // Komisyon hesaplama ve hakediş kalemi oluşturma artık ödeme anında değil,
                        // sipariş teslim edildiğinde (Delivered) yapılır — bkz. CreateSettlementForDeliveredOrder.
                        _unitOfWork.PaymentRepository.Update(payment);
                    }

                    order.PaymentStatusId = (short)PaymentStatusEnums.Completed;
                    order.StatusId = (short)OrderStatusEnums.WaitingRestaurantApproval;
                    _unitOfWork.OrderRepository.Update(order);
                    await AddStatusHistory(order.Id, OrderStatusEnums.WaitingRestaurantApproval);

                    // Kart kaydedildiyse CardUserKey ve kart bilgilerini UserExternalInfo'ya yaz
                    if (!string.IsNullOrEmpty(completeResult.Data.CardUserKey))
                    {
                        var existingKey = await _context.UserExternalInfos
                            .FirstOrDefaultAsync(x => x.UserId == order.UserId && x.Provider == "iyzico" && x.Key == "CardUserKey");
                        if (existingKey == null)
                            _context.UserExternalInfos.Add(new UserExternalInfo
                            {
                                Id = Guid.NewGuid(),
                                UserId = order.UserId,
                                Provider = "iyzico",
                                Key = "CardUserKey",
                                Value = completeResult.Data.CardUserKey
                            });

                        // Kart alias bilgisini kaydet
                        if (!string.IsNullOrEmpty(completeResult.Data.CardToken) && payment != null)
                        {
                            var alias = !string.IsNullOrEmpty(payment.CardAlias) ? payment.CardAlias : "Kartım";
                            var cardInfoJson = System.Text.Json.JsonSerializer.Serialize(new
                            {
                                cardToken = completeResult.Data.CardToken,
                                alias = alias,
                                binNumber = completeResult.Data.BinNumber,
                                lastFourDigits = completeResult.Data.LastFourDigits,
                                cardType = completeResult.Data.CardType,
                                cardAssociation = completeResult.Data.CardAssociation
                            });
                            _context.UserExternalInfos.Add(new UserExternalInfo
                            {
                                Id = Guid.NewGuid(),
                                UserId = order.UserId,
                                Provider = "iyzico",
                                Key = $"SavedCard:{completeResult.Data.CardToken}",
                                Value = cardInfoJson
                            });
                        }
                    }

                    await _context.SaveChangesAsync();

                    // Notify restaurant about new order (Hangfire creates its own DI scope)
                    BackgroundJob.Enqueue<IPaymentService>(s =>
                        s.NotifyRestaurantNewOrderBackground(order.Id, order.RestaurantId, order.TotalPrice));

                    result.SetData(true);
                }
                else
                {
                    // 3DS tamamlama başarısız
                    var errMsg = completeResult.Messages?.FirstOrDefault()?.Description ?? "Ödeme tamamlanamadı.";
                    await FailPaymentAndOrder(payment, order, paymentId, errMsg);
                    await _context.SaveChangesAsync();
                    result.Fail(errMsg);
                }
            }
            else
            {
                // Callback başarısız geldi
                await FailPaymentAndOrder(payment, order, paymentId, errorMessage ?? "Ödeme başarısız oldu.");
                await _context.SaveChangesAsync();
                result.Fail(errorMessage ?? "Ödeme başarısız oldu.");
            }
        }
        catch (Exception e)
        {
            result.Fail(e);
        }

        return result;
    }

    public async Task<ServiceObjectResult<bool>> RefundOrderAsync(Guid orderId, string? reason = null)
    {
        var result = new ServiceObjectResult<bool>();
        try
        {
            var payment = await _unitOfWork.PaymentRepository.GetAsync(
                x => x.OrderId == orderId && x.StatusId == (short)PaymentStatusEnums.Completed,
                enableTracking: true);

            if (payment == null)
            {
                result.Fail("Iade edilecek tamamlanmış ödeme bulunamadı.");
                return result;
            }

            if (payment.RefundedAt != null)
            {
                result.Fail("Bu ödeme zaten iade edilmiş.");
                return result;
            }

            // Call iyzico refund
            var refundResult = _iyzicoAdapter.RefundPayment(
                payment.ProviderPaymentId ?? payment.ProviderConversationId ?? "",
                payment.Amount);

            if (refundResult.HasFailed || refundResult.Data?.Success != true)
            {
                var errMsg = refundResult.Data?.ErrorMessage ?? refundResult.Messages?.FirstOrDefault()?.Description ?? "Iyzico iade başarısız.";
                result.Fail(errMsg);
                return result;
            }

            payment.StatusId = (short)PaymentStatusEnums.Refunded;
            payment.RefundedAt = DateTime.UtcNow;
            payment.RefundTransactionId = refundResult.Data.TransactionId;
            payment.RefundReason = reason;
            _unitOfWork.PaymentRepository.Update(payment);
            await _context.SaveChangesAsync();

            // Notify customer (Hangfire creates its own DI scope)
            BackgroundJob.Enqueue<INotificationService>(s =>
                s.SendToUserAsync(payment.UserId, "Iade Bilgisi",
                    $"Siparişiniz için {payment.Amount:F2} TL iade yapıldı.",
                    new Dictionary<string, string> { { "orderId", orderId.ToString() } }));

            result.SetData(true);
        }
        catch (Exception e)
        {
            result.Fail(e);
        }

        return result;
    }

    public async Task NotifyRestaurantNewOrderBackground(Guid orderId, Guid restaurantId, decimal totalPrice)
    {
        try
        {
            var restaurant = await _context.Set<Domain.Entities.Seller.Restaurant>()
                .FirstOrDefaultAsync(r => r.Id == restaurantId);
            if (restaurant != null)
            {
                var sellerUser = await _context.Set<User>()
                    .FirstOrDefaultAsync(u => u.SellerId == restaurant.SellerId);
                if (sellerUser != null)
                    await _notificationService.SendToUserAsync(sellerUser.Id, "Yeni Sipariş",
                        $"Yeni sipariş #{orderId.ToString()[..8]}",
                        new Dictionary<string, string> { { "orderId", orderId.ToString() } });
            }

            await _realtimeNotifier.NotifyNewOrderToRestaurant(restaurantId, orderId, totalPrice);
            await _realtimeNotifier.NotifyOrderStatusChanged(orderId,
                (short)OrderStatusEnums.WaitingRestaurantApproval);
        }
        catch
        {
            /* best effort — background job, no caller to report to */
        }
    }

    private async Task FailPaymentAndOrder(Payment? payment, Order order, string? paymentId, string errorMessage)
    {
        if (payment != null)
        {
            payment.StatusId = (short)PaymentStatusEnums.Failed;
            payment.ProviderPaymentId = paymentId;
            payment.ErrorMessage = errorMessage;
            payment.FailedAt = DateTime.UtcNow;
            _unitOfWork.PaymentRepository.Update(payment);
        }

        order.PaymentStatusId = (short)PaymentStatusEnums.Failed;
        order.StatusId = (short)OrderStatusEnums.PaymentFailed;
        order.CancellationReason = $"Ödeme başarısız: {errorMessage}";
        _unitOfWork.OrderRepository.Update(order);
        await AddStatusHistory(order.Id, OrderStatusEnums.PaymentFailed, errorMessage);
    }

    private async Task AddStatusHistory(Guid orderId, OrderStatusEnums status, string? note = null)
    {
        var history = new OrderStatusHistory
        {
            Id = Guid.NewGuid(),
            OrderId = orderId,
            StatusId = (short)status,
            Note = note,
            OccurredAt = DateTime.UtcNow
        };
        await _unitOfWork.OrderStatusHistoryRepository.AddAsync(history);
    }
}