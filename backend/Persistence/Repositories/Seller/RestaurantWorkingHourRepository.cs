using Domain.Entities.Seller;
using NArchitecture.Core.Persistence.Repositories;
using Persistence.Contexts;
using Persistence.IRepositories.Seller;

namespace Persistence.Repositories.Seller;

public class RestaurantWorkingHourRepository : EfRepositoryBase<RestaurantWorkingHour, Guid, BaseDbContext>, IRestaurantWorkingHourRepository
{
    public RestaurantWorkingHourRepository(BaseDbContext context) : base(context)
    {
    }
}