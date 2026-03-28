using Domain.Entities.Seller;
using NArchitecture.Core.Persistence.Repositories;
using Persistence.Contexts;
using Persistence.IRepositories.Seller;

namespace Persistence.Repositories.Seller;

public class RestaurantCommissionRepository : EfRepositoryBase<RestaurantCommission, Guid, BaseDbContext>, IRestaurantCommissionRepository
{
    public RestaurantCommissionRepository(BaseDbContext context) : base(context)
    {
    }
}
