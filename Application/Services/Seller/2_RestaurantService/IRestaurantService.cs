using Domain.Dto.Seller.Restaurant;
using Domain.Service;

namespace Application.Services.Seller._2_RestaurantService;

public interface IRestaurantService
{
    Task<ServiceObjectResult<Guid>> AddRestaurant(AddRestaurantDto requestDto);
    Task<ServiceObjectResult<string>> GetRestaurantInfo(GetRestaurantInformationRequestDto requestDto);
    Task<ServiceCollectionResult<GetRestaurantListForSellerResponseDto>> GetRestaurantListForSeller();
    Task<ServiceObjectResult<string>> GetRestaurantInfoForSeller(GetRestaurantInformationRequestDto requestDto);
}