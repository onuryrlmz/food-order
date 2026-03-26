using Domain.Dto.Buyer.ScheduledOrder;
using Domain.Service;

namespace Application.Services.Buyer.ScheduledOrderService;

public interface IScheduledOrderService
{
    Task<ServiceObjectResult<ScheduledOrderDto>> CreateScheduledOrder(CreateScheduledOrderRequestDto requestDto);
    Task<ServiceCollectionResult<ScheduledOrderDto>> GetMyScheduledOrders(int page = 1, int pageSize = 20);
    Task<ServiceObjectResult<ScheduledOrderDetailDto>> GetScheduledOrderDetail(Guid scheduledOrderId);
    Task<ServiceObjectResult<bool>> CancelScheduledOrder(Guid scheduledOrderId, string? reason);
    Task<ServiceObjectResult<bool>> UpdateScheduledOrder(Guid scheduledOrderId, UpdateScheduledOrderRequestDto requestDto);
    Task ProcessDueScheduledOrders();
}
