using Domain.Entities.Seller;
using NArchitecture.Core.Persistence.Repositories;
using Persistence.Contexts;
using Persistence.IRepositories.Seller;

namespace Persistence.Repositories.Seller;

public class RestaurantCdnUpdateQueueRepository : EfRepositoryBase<RestaurantCdnUpdateQueue, Guid, BaseDbContext>, IRestaurantCdnUpdateQueueRepository
{
    public RestaurantCdnUpdateQueueRepository(BaseDbContext context) : base(context)
    {
    }
}