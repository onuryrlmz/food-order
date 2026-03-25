using Domain.Entities.Seller;
using NArchitecture.Core.Persistence.Repositories;
using Persistence.Contexts;
using Persistence.IRepositories.Seller;

namespace Persistence.Repositories.Seller;

public class SubscriptionRepository : EfRepositoryBase<Subscription, Guid, BaseDbContext>, ISubscriptionRepository
{
    public SubscriptionRepository(BaseDbContext context) : base(context)
    {
    }
}