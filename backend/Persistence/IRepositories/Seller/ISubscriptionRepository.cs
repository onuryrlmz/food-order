using Domain.Entities.Seller;
using NArchitecture.Core.Persistence.Repositories;

namespace Persistence.IRepositories.Seller;

public interface ISubscriptionRepository : IAsyncRepository<Subscription, Guid>, IRepository<Subscription, Guid>
{
}