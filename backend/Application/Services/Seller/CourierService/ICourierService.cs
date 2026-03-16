using Domain.Dto.Seller.Courier;
using Domain.Service;

namespace Application.Services.Seller.CourierService;

public interface ICourierService
{
    Task<ServiceObjectResult<bool>> AddCourierAsync(Guid restaurantId, string email);
    Task<ServiceCollectionResult<RestaurantCourierDto>> GetRestaurantCouriersAsync(Guid restaurantId);
    Task<ServiceObjectResult<bool>> RemoveCourierAsync(Guid restaurantId, Guid courierId);
}
