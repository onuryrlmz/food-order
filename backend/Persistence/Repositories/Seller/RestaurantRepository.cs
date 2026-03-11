using Domain.Entities.Seller;
using NArchitecture.Core.Persistence.Repositories;
using Persistence.Contexts;
using Persistence.IRepositories.Seller;

namespace Persistence.Repositories.Seller;

public class RestaurantRepository : EfRepositoryBase<Restaurant, Guid, BaseDbContext>, IRestaurantRepository
{
    public RestaurantRepository(BaseDbContext context) : base(context)
    {
    }
}