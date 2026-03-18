using Domain.Dto.Admin.Order;
using Domain.Dto.Buyer.Order;
using Domain.Service;

namespace Application.Services.Buyer.OrderService;

public interface IOrderService
{
    Task<ServiceObjectResult<PlaceOrderResponseDto>> PlaceOrder(PlaceOrderRequestDto requestDto);
    Task<ServiceObjectResult<GetOrderResponseDto>> GetOrderById(Guid orderId);
    Task<ServiceCollectionResult<GetOrderResponseDto>> GetActiveOrders();
    Task<ServiceCollectionResult<GetOrderResponseDto>> GetOrderHistory(int page = 1, int pageSize = 20);
    Task<ServiceObjectResult<bool>> CancelOrder(Guid orderId, string reason);
    Task<ServiceObjectResult<bool>> UpdateOrderStatus(Guid orderId, short statusId);
    Task<ServiceCollectionResult<GetOrderResponseDto>> GetRestaurantOrders(Guid restaurantId, short? statusId = null, int page = 1, int pageSize = 20);
    Task<ServiceCollectionResult<AdminGetOrderResponseDto>> GetAllOrdersForAdmin(int page = 1, int pageSize = 20, short? statusId = null);
    Task<ServiceObjectResult<AdminGetOrderResponseDto>> GetOrderDetailForAdmin(Guid orderId);
    Task<ServiceObjectResult<InitiatePaymentResponseDto>> InitiatePayment(Guid orderId, InitiatePaymentRequestDto requestDto);
    Task<ServiceCollectionResult<AdminGetOrderResponseDto>> GetOverdueOrdersForAdmin(int page = 1, int pageSize = 20);
    Task<ServiceObjectResult<ReorderResponseDto>> ReorderAsync(Guid orderId);
}
