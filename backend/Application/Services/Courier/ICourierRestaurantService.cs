using Domain.Dto.Courier;
using Domain.Service;

namespace Application.Services.Courier;

public interface ICourierRestaurantService
{
    Task<ServiceCollectionResult<CourierRestaurantDto>> GetMyRestaurantsAsync(Guid userId);
    Task<ServiceCollectionResult<CourierRestaurantDto>> GetPendingInvitesAsync(Guid userId);
    Task<ServiceObjectResult<bool>> AcceptInviteAsync(Guid userId, Guid restaurantCourierId);
    Task<ServiceObjectResult<bool>> RejectInviteAsync(Guid userId, Guid restaurantCourierId);
    Task<ServiceObjectResult<bool>> LeaveRestaurantAsync(Guid userId, Guid restaurantCourierId);
}
