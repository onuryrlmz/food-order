using Domain.Entities.Buyer;
using NArchitecture.Core.Persistence.Repositories;

namespace Persistence.IRepositories.Buyer;

public interface IScheduledOrderRepository : IAsyncRepository<ScheduledOrder, Guid>, IRepository<ScheduledOrder, Guid>
{
}
