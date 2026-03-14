using Base.Enums;
using Domain.Entities.Buyer;
using Domain.Entities.Common;
using Domain.Service;
using Infrastructure.Adapters.IyzicoServiceAdapter;
using Microsoft.EntityFrameworkCore;
using Persistence.Contexts;
using Persistence.IRepositories;

namespace Application.Services.Buyer.PaymentService;

public class PaymentManager : IPaymentService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IIyzicoServiceAdapter _iyzicoAdapter;
    private readonly BaseDbContext _context;

    public PaymentManager(
        IUnitOfWork unitOfWork,
        IIyzicoServiceAdapter iyzicoAdapter,
        BaseDbContext context)
    {
        _unitOfWork = unitOfWork;
        _iyzicoAdapter = iyzicoAdapter;
        _context = context;
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
