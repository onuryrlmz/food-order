using Domain.Entities.Common;
using NArchitecture.Core.Persistence.Repositories;

namespace Persistence.IRepositories.Common;

public interface IPlatformCommissionScheduleRepository : IAsyncRepository<PlatformCommissionSchedule, Guid>, IRepository<PlatformCommissionSchedule, Guid>
{
}
