using Domain.Entities.Buyer;
using NArchitecture.Core.Persistence.Repositories;
using Persistence.Contexts;
using Persistence.IRepositories.Buyer;

namespace Persistence.Repositories.Buyer;

public class ScheduledOrderRepository : EfRepositoryBase<ScheduledOrder, Guid, BaseDbContext>, IScheduledOrderRepository
{
    public ScheduledOrderRepository(BaseDbContext context) : base(context)
    {
    }
}
