using Domain.Entities.Seller;
using NArchitecture.Core.Persistence.Repositories;

namespace Persistence.IRepositories.Seller;

public interface IRestaurantCommissionRepository : IAsyncRepository<RestaurantCommission, Guid>, IRepository<RestaurantCommission, Guid>
{
}
