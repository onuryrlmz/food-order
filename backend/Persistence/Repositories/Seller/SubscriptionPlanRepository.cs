using Domain.Entities.Seller;
using NArchitecture.Core.Persistence.Repositories;
using Persistence.Contexts;
using Persistence.IRepositories.Seller;

namespace Persistence.Repositories.Seller;

public class SubscriptionPlanRepository : EfRepositoryBase<SubscriptionPlan, Guid, BaseDbContext>, ISubscriptionPlanRepository
{
    public SubscriptionPlanRepository(BaseDbContext context) : base(context)
    {
    }
}
