using Domain.Dto.Courier;
using Domain.Service;

namespace Application.Services.Courier;

public interface ICourierOrderService
{
    Task<ServiceCollectionResult<CourierOrderDto>> GetActiveOrdersAsync(Guid courierId);
    Task<ServiceCollectionResult<CourierOrderDto>> GetOrderHistoryAsync(Guid courierId, int month, int year);
}
