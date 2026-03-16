using Domain.Entities.Courier;
using NArchitecture.Core.Persistence.Repositories;
using Persistence.Contexts;
using Persistence.IRepositories.Courier;

namespace Persistence.Repositories.Courier;

public class RestaurantCourierRepository : EfRepositoryBase<RestaurantCourier, Guid, BaseDbContext>, IRestaurantCourierRepository
{
    public RestaurantCourierRepository(BaseDbContext context) : base(context)
    {
    }
}
