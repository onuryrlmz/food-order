using Domain.Dto.Seller.Restaurant;
using Domain.Service;

namespace Application.Services.Seller.RestaurantService;

public interface IRestaurantService
{
    Task<ServiceObjectResult<Guid>> AddRestaurant(AddRestaurantDto requestDto);
    Task<ServiceCollectionResult<GetRestaurantsResponseDto>> GetRestaurants(GetRestaurantsRequestDto requestDto);
    Task<ServiceObjectResult<string>> GetRestaurantInfo(GetRestaurantInformationRequestDto requestDto);
    Task<ServiceCollectionResult<GetRestaurantListForSellerResponseDto>> GetRestaurantListForSeller();
    Task<ServiceObjectResult<string>> GetRestaurantInfoForSeller(GetRestaurantInformationRequestDto requestDto);
    Task<ServiceObjectResult<bool>> UpdateRestaurant(UpdateRestaurantDto requestDto);
    Task<ServiceObjectResult<bool>> ToggleOpen(Guid restaurantId);
    Task<ServiceObjectResult<bool>> ToggleActive(Guid restaurantId);
    Task<ServiceObjectResult<bool>> ApproveRestaurant(Guid restaurantId);
    Task<ServiceCollectionResult<GetAdminRestaurantListResponseDto>> GetAllRestaurantsForAdmin(int page = 1, int pageSize = 50);
    Task<ServiceObjectResult<GetAdminRestaurantListResponseDto>> GetRestaurantByIdForAdmin(Guid id);
    Task<ServiceObjectResult<bool>> UpdateRestaurantForAdmin(UpdateAdminRestaurantDto requestDto);
    Task<ServiceCollectionResult<WorkingHourDto>> GetWorkingHours(Guid restaurantId);
    Task<ServiceObjectResult<bool>> UpsertWorkingHour(UpsertWorkingHourDto requestDto);
}