using Domain.Dto.Courier;
using Domain.Service;

namespace Application.Services.Courier;

public interface ICourierLocationService
{
    Task<ServiceObjectResult<bool>> UpdateLocationAsync(Guid courierId, decimal latitude, decimal longitude, Guid? orderId = null);
}
