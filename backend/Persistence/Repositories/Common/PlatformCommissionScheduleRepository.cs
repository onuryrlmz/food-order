using Domain.Entities.Common;
using NArchitecture.Core.Persistence.Repositories;
using Persistence.Contexts;
using Persistence.IRepositories.Common;

namespace Persistence.Repositories.Common;

public class PlatformCommissionScheduleRepository : EfRepositoryBase<PlatformCommissionSchedule, Guid, BaseDbContext>, IPlatformCommissionScheduleRepository
{
    public PlatformCommissionScheduleRepository(BaseDbContext context) : base(context)
    {
    }
}
