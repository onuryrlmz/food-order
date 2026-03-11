using Domain.Entities.Buyer;
using NArchitecture.Core.Persistence.Repositories;

namespace Persistence.IRepositories.Buyer;

public interface IOrderItemRepository : IAsyncRepository<OrderItem, Guid>, IRepository<OrderItem, Guid>
{
}
