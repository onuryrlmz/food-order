using Domain.Entities.Seller;
using NArchitecture.Core.Persistence.Repositories;
using Persistence.Contexts;
using Persistence.IRepositories.Seller;

namespace Persistence.Repositories.Seller;

public class SubscriptionUsageRepository : EfRepositoryBase<SubscriptionUsage, Guid, BaseDbContext>, ISubscriptionUsageRepository
{
    public SubscriptionUsageRepository(BaseDbContext context) : base(context)
    {
    }
}
