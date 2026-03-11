using Domain.Entities.Seller;
using NArchitecture.Core.Persistence.Repositories;

namespace Persistence.IRepositories.Seller;

public interface IScheduledTaskRepository : IAsyncRepository<ScheduledTask, Guid>, IRepository<ScheduledTask, Guid>
{
}