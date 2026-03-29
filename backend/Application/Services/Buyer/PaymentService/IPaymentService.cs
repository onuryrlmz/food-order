using Domain.Dto.Buyer.Order;
using Domain.Entities.Buyer;
using Domain.Service;

namespace Application.Services.Buyer.PaymentService;

public interface IPaymentService
{
    Task<Payment> CreatePendingPayment(Guid orderId, Guid userId, Guid sellerId, decimal amount, int paymentOptionId, string? providerConversationId, string? cardAlias = null);
    Task<ServiceObjectResult<bool>> HandlePaymentCallback(string conversationId, string paymentId, string conversationData, bool isSuccess, string? errorMessage = null);
    Task<ServiceObjectResult<bool>> RefundOrderAsync(Guid orderId, string? reason = null);

    // Background-callable methods (invoked via Hangfire — must accept only serializable args)
    Task NotifyRestaurantNewOrderBackground(Guid orderId, Guid restaurantId, decimal totalPrice);
}