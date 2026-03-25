using Domain.Entities.Buyer;
using NArchitecture.Core.Persistence.Repositories;
using Persistence.Contexts;
using Persistence.IRepositories.Buyer;

namespace Persistence.Repositories.Buyer;

public class FavoriteRestaurantRepository : EfRepositoryBase<FavoriteRestaurant, Guid, BaseDbContext>, IFavoriteRestaurantRepository
{
    public FavoriteRestaurantRepository(BaseDbContext context) : base(context)
    {
    }
}