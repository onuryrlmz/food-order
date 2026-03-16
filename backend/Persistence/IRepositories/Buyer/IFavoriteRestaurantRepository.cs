using Domain.Entities.Buyer;
using NArchitecture.Core.Persistence.Repositories;

namespace Persistence.IRepositories.Buyer;

public interface IFavoriteRestaurantRepository : IAsyncRepository<FavoriteRestaurant, Guid>, IRepository<FavoriteRestaurant, Guid>
{
}
