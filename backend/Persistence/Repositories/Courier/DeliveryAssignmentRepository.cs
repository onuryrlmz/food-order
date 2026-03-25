using Domain.Entities.Courier;
using NArchitecture.Core.Persistence.Repositories;
using Persistence.Contexts;
using Persistence.IRepositories.Courier;

namespace Persistence.Repositories.Courier;

public class DeliveryAssignmentRepository : EfRepositoryBase<DeliveryAssignment, Guid, BaseDbContext>, IDeliveryAssignmentRepository
{
    public DeliveryAssignmentRepository(BaseDbContext context) : base(context)
    {
    }
}
