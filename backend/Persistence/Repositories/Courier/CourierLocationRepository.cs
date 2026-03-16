using Domain.Entities.Courier;
using NArchitecture.Core.Persistence.Repositories;
using Persistence.Contexts;
using Persistence.IRepositories.Courier;

namespace Persistence.Repositories.Courier;

public class CourierLocationRepository : EfRepositoryBase<CourierLocation, Guid, BaseDbContext>, ICourierLocationRepository
{
    public CourierLocationRepository(BaseDbContext context) : base(context)
    {
    }
}
