using Domain.Dto.Buyer.Order;
using Domain.Service;

namespace Application.Services.Buyer.OrderService;

public interface IOrderService
{
    Task<ServiceObjectResult<Guid>> PlaceOrder(PlaceOrderRequestDto requestDto);
    Task<ServiceObjectResult<GetOrderResponseDto>> GetOrderById(Guid orderId);
    Task<ServiceCollectionResult<GetOrderResponseDto>> GetOrderHistory(int page = 1, int pageSize = 20);
    Task<ServiceObjectResult<bool>> CancelOrder(Guid orderId, string reason);
    Task<ServiceObjectResult<bool>> UpdateOrderStatus(Guid orderId, short statusId);
    Task<ServiceCollectionResult<GetOrderResponseDto>> GetRestaurantOrders(Guid restaurantId, short? statusId = null, int page = 1, int pageSize = 20);
    Task<ServiceCollectionResult<GetOrderResponseDto>> GetAllOrdersForAdmin(int page = 1, int pageSize = 20, short? statusId = null);
    Task<ServiceObjectResult<InitiatePaymentResponseDto>> InitiatePayment(Guid orderId, InitiatePaymentRequestDto requestDto);
}
