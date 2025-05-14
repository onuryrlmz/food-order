using Domain.Entities.Seller;
using NArchitecture.Core.Persistence.Repositories;
using Persistence.Contexts;
using Persistence.IRepositories.Seller;

namespace Persistence.Repositories.Seller;

public class ScheduledTaskRepository : EfRepositoryBase<ScheduledTask, Guid, BaseDbContext>, IScheduledTaskRepository
{
    public ScheduledTaskRepository(BaseDbContext context) : base(context)
    {
    }
}