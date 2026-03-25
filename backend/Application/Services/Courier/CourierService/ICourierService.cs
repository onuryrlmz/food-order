using Domain.Dto.Courier;
using Domain.Dto.Admin.Courier;
using Domain.Service;

namespace Application.Services.Courier.CourierService;

public interface ICourierService
{
    // Courier self-service
    Task<ServiceObjectResult<CourierProfileResponseDto>> Register(RegisterCourierRequestDto requestDto);
    Task<ServiceObjectResult<CourierProfileResponseDto>> GetProfile();
    Task<ServiceObjectResult<bool>> UpdateProfile(UpdateCourierProfileRequestDto requestDto);
    Task<ServiceObjectResult<bool>> GoOnline();
    Task<ServiceObjectResult<bool>> GoOffline();
    Task<ServiceObjectResult<bool>> UpdateLocation(UpdateLocationRequestDto requestDto);

    // Admin
    Task<ServiceCollectionResult> GetAllCouriersForAdmin(int page = 1, int pageSize = 20, short? statusId = null);
    Task<ServiceObjectResult<bool>> ApproveCourier(Guid courierId);
    Task<ServiceObjectResult<bool>> SuspendCourier(Guid courierId);

    // Seller - get available couriers for restaurant
    Task<ServiceCollectionResult> GetAvailableCouriersForRestaurant(Guid restaurantId);
}