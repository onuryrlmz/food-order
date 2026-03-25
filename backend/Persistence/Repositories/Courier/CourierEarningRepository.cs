using Domain.Entities.Courier;
using NArchitecture.Core.Persistence.Repositories;
using Persistence.Contexts;
using Persistence.IRepositories.Courier;

namespace Persistence.Repositories.Courier;

public class CourierEarningRepository : EfRepositoryBase<CourierEarning, Guid, BaseDbContext>, ICourierEarningRepository
{
    public CourierEarningRepository(BaseDbContext context) : base(context)
    {
    }
}
