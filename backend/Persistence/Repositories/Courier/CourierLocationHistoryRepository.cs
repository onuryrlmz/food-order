using NArchitecture.Core.Persistence.Repositories;
using Persistence.Contexts;
using Persistence.IRepositories.Courier;

namespace Persistence.Repositories.Courier;

public class CourierLocationHistoryRepository : EfRepositoryBase<Domain.Entities.Courier.CourierLocationHistory, Guid, BaseDbContext>, ICourierLocationHistoryRepository
{
    public CourierLocationHistoryRepository(BaseDbContext context) : base(context)
    {
    }
}
