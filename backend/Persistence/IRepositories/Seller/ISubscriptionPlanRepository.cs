using Domain.Entities.Seller;
using NArchitecture.Core.Persistence.Repositories;

namespace Persistence.IRepositories.Seller;

public interface ISubscriptionPlanRepository : IAsyncRepository<SubscriptionPlan, Guid>, IRepository<SubscriptionPlan, Guid>
{
}
