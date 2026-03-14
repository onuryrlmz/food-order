using Domain.Entities.Buyer;
using NArchitecture.Core.Persistence.Repositories;

namespace Persistence.IRepositories.Buyer;

public interface IOrderStatusHistoryRepository : IAsyncRepository<OrderStatusHistory, Guid>, IRepository<OrderStatusHistory, Guid>
{
}
