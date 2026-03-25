using Domain.Dto.Buyer.Favorite;
using Domain.Service;

namespace Application.Services.Buyer.FavoriteService;

public interface IFavoriteService
{
    Task<ServiceObjectResult<bool>> AddFavorite(Guid restaurantId);
    Task<ServiceObjectResult<bool>> RemoveFavorite(Guid restaurantId);
    Task<ServiceCollectionResult<FavoriteRestaurantDto>> GetFavorites(int page = 1, int pageSize = 20);
    Task<ServiceObjectResult<HashSet<Guid>>> GetFavoriteRestaurantIds();
}