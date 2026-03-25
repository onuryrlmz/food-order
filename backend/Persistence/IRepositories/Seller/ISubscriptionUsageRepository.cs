using Domain.Entities.Seller;
using NArchitecture.Core.Persistence.Repositories;

namespace Persistence.IRepositories.Seller;

public interface ISubscriptionUsageRepository : IAsyncRepository<SubscriptionUsage, Guid>, IRepository<SubscriptionUsage, Guid>
{
}