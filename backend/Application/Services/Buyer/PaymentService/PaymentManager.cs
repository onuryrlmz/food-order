using Base.Enums;
using Domain.Entities.Buyer;
using Domain.Entities.Common;
using Domain.Service;
using Application.Services.Common.NotificationService;
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

    public PaymentManager(
        IUnitOfWork unitOfWork,
        IIyzicoServiceAdapter iyzicoAdapter,
        BaseDbContext context,
        INotificationService notificationService,
        IRealtimeNotifier realtimeNotifier)
    {
        _unitOfWork = unitOfWork;
        _iyzicoAdapter = iyzicoAdapter;
        _context = context;
        _notificationService = notificationService;
        _realtimeNotifier = realtimeNotifier;
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
            StatusId = (short)AuthorizationServiceEnums.PaymentStatusEnums.Pending,
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
                x => x.OrderId == orderId && x.StatusId == (short)AuthorizationServiceEnums.PaymentStatusEnums.Pending,
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
                        payment.StatusId = (short)AuthorizationServiceEnums.PaymentStatusEnums.Completed;
                        payment.ProviderPaymentId = paymentId;
                        payment.CompletedAt = DateTime.UtcNow;

                        // Commission calculation
                        var subscription = await _context.Set<Domain.Entities.Seller.Subscription>()
                            .Include(s => s.SubscriptionPlan)
                            .FirstOrDefaultAsync(s => s.RestaurantId == order.RestaurantId
                                && s.StatusId == (short)AuthorizationServiceEnums.SubscriptionStatusEnums.Active);
                        var commissionRate = subscription?.SubscriptionPlan?.CommissionRate ?? 0.10m;
                        payment.CommissionAmount = payment.Amount * commissionRate;
                        payment.SellerPayoutAmount = payment.Amount - payment.CommissionAmount;

                        _unitOfWork.PaymentRepository.Update(payment);
                    }

                    order.PaymentStatusId = (short)AuthorizationServiceEnums.PaymentStatusEnums.Completed;
                    order.StatusId = (short)AuthorizationServiceEnums.OrderStatusEnums.WaitingRestaurantApproval;
                    _unitOfWork.OrderRepository.Update(order);
                    await AddStatusHistory(order.Id, AuthorizationServiceEnums.OrderStatusEnums.WaitingRestaurantApproval);

                    // Kart kaydedildiyse CardUserKey ve kart bilgilerini UserExternalInfo'ya yaz
                    if (!string.IsNullOrEmpty(completeResult.Data.CardUserKey))
                    {
                        var existingKey = await _context.UserExternalInfos
                            .FirstOrDefaultAsync(x => x.UserId == order.UserId && x.Provider == "iyzico" && x.Key == "CardUserKey");
                        if (existingKey == null)
                        {
                            _context.UserExternalInfos.Add(new UserExternalInfo
                            {
                                Id = Guid.NewGuid(),
                                UserId = order.UserId,
                                Provider = "iyzico",
                                Key = "CardUserKey",
                                Value = completeResult.Data.CardUserKey
                            });
                        }

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

                    // Notify restaurant about new order
                    _ = Task.Run(async () =>
                    {
                        try
                        {
                            // Find seller user for push notification
                            var restaurant = await _context.Set<Domain.Entities.Seller.Restaurant>()
                                .FirstOrDefaultAsync(r => r.Id == order.RestaurantId);
                            if (restaurant != null)
                            {
                                var sellerUser = await _context.Set<Domain.Entities.Common.User>()
                                    .FirstOrDefaultAsync(u => u.SellerId == restaurant.SellerId);
                                if (sellerUser != null)
                                {
                                    await _notificationService.SendToUserAsync(sellerUser.Id, "Yeni Sipariş",
                                        $"Yeni sipariş #{order.Id.ToString()[..8]}",
                                        new Dictionary<string, string> { { "orderId", order.Id.ToString() } });
                                }
                            }

                            await _realtimeNotifier.NotifyNewOrderToRestaurant(order.RestaurantId, order.Id, order.TotalPrice);
                            await _realtimeNotifier.NotifyOrderStatusChanged(order.Id,
                                (short)AuthorizationServiceEnums.OrderStatusEnums.WaitingRestaurantApproval);
                        }
                        catch { /* best effort */ }
                    });

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
                x => x.OrderId == orderId && x.StatusId == (short)AuthorizationServiceEnums.PaymentStatusEnums.Completed,
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

            payment.StatusId = (short)AuthorizationServiceEnums.PaymentStatusEnums.Refunded;
            payment.RefundedAt = DateTime.UtcNow;
            payment.RefundTransactionId = refundResult.Data.TransactionId;
            payment.RefundReason = reason;
            _unitOfWork.PaymentRepository.Update(payment);
            await _context.SaveChangesAsync();

            // Notify customer
            _ = Task.Run(async () =>
            {
                try
                {
                    await _notificationService.SendToUserAsync(payment.UserId, "Iade Bilgisi",
                        $"Siparişiniz için {payment.Amount:F2} TL iade yapıldı.",
                        new Dictionary<string, string> { { "orderId", orderId.ToString() } });
                }
                catch { /* best effort */ }
            });

            result.SetData(true);
        }
        catch (Exception e)
        {
            result.Fail(e);
        }

        return result;
    }

    private async Task FailPaymentAndOrder(Payment? payment, Order order, string? paymentId, string errorMessage)
    {
        if (payment != null)
        {
            payment.StatusId = (short)AuthorizationServiceEnums.PaymentStatusEnums.Failed;
            payment.ProviderPaymentId = paymentId;
            payment.ErrorMessage = errorMessage;
            payment.FailedAt = DateTime.UtcNow;
            _unitOfWork.PaymentRepository.Update(payment);
        }

        order.PaymentStatusId = (short)AuthorizationServiceEnums.PaymentStatusEnums.Failed;
        order.StatusId = (short)AuthorizationServiceEnums.OrderStatusEnums.PaymentFailed;
        order.CancellationReason = $"Ödeme başarısız: {errorMessage}";
        _unitOfWork.OrderRepository.Update(order);
        await AddStatusHistory(order.Id, AuthorizationServiceEnums.OrderStatusEnums.PaymentFailed, errorMessage);
    }

    private async Task AddStatusHistory(Guid orderId, AuthorizationServiceEnums.OrderStatusEnums status, string? note = null)
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
