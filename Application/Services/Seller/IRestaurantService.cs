using Domain.Dto.Seller;
using Domain.Service;

namespace Application.Services.Seller;

public interface IRestaurantService
{
    Task<ServiceObjectResult<Guid>> AddRestaurant(AddRestaurantDto requestDto);
    Task<ServiceObjectResult<bool>> CreateRestaurantInformationJsonFile(CreateRestaurantInformationJsonFileRequestDto requestDto);
    Task<ServiceObjectResult<RestaurantResponseDto>> GetRestaurantInformationByRestaurantId(GetRestaurantInformationRequestDto requestDto);
}