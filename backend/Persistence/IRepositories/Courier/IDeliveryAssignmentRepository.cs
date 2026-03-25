using Domain.Entities.Courier;
using NArchitecture.Core.Persistence.Repositories;

namespace Persistence.IRepositories.Courier;

public interface IDeliveryAssignmentRepository : IAsyncRepository<DeliveryAssignment, Guid>, IRepository<DeliveryAssignment, Guid>
{
}