using Domain.Entities.Courier;
using NArchitecture.Core.Persistence.Repositories;
using Persistence.Contexts;
using Persistence.IRepositories.Courier;

namespace Persistence.Repositories.Courier;

public class RestaurantCourierAgreementRepository : EfRepositoryBase<RestaurantCourierAgreement, Guid, BaseDbContext>, IRestaurantCourierAgreementRepository
{
    public RestaurantCourierAgreementRepository(BaseDbContext context) : base(context)
    {
    }
}