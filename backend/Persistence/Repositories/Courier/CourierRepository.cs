using NArchitecture.Core.Persistence.Repositories;
using Persistence.Contexts;
using Persistence.IRepositories.Courier;

namespace Persistence.Repositories.Courier;

public class CourierRepository : EfRepositoryBase<Domain.Entities.Courier.Courier, Guid, BaseDbContext>, ICourierRepository
{
    public CourierRepository(BaseDbContext context) : base(context)
    {
    }
}
