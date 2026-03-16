using Domain.Entities.Courier;
using NArchitecture.Core.Persistence.Repositories;
using Persistence.Contexts;
using Persistence.IRepositories.Courier;

namespace Persistence.Repositories.Courier;

public class RestaurantCourierCompanyRepository : EfRepositoryBase<RestaurantCourierCompany, Guid, BaseDbContext>, IRestaurantCourierCompanyRepository
{
    public RestaurantCourierCompanyRepository(BaseDbContext context) : base(context)
    {
    }
}
