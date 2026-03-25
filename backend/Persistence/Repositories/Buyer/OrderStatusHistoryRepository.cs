using Domain.Entities.Buyer;
using NArchitecture.Core.Persistence.Repositories;
using Persistence.Contexts;
using Persistence.IRepositories.Buyer;

namespace Persistence.Repositories.Buyer;

public class OrderStatusHistoryRepository : EfRepositoryBase<OrderStatusHistory, Guid, BaseDbContext>, IOrderStatusHistoryRepository
{
    public OrderStatusHistoryRepository(BaseDbContext context) : base(context)
    {
    }
}