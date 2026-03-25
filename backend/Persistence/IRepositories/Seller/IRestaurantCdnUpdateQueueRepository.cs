using Domain.Entities.Seller;
using NArchitecture.Core.Persistence.Repositories;

namespace Persistence.IRepositories.Seller;

public interface IRestaurantCdnUpdateQueueRepository : IAsyncRepository<RestaurantCdnUpdateQueue, Guid>, IRepository<RestaurantCdnUpdateQueue, Guid>
{
}