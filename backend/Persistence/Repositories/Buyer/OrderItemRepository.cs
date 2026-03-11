using Domain.Entities.Buyer;
using NArchitecture.Core.Persistence.Repositories;
using Persistence.Contexts;
using Persistence.IRepositories.Buyer;

namespace Persistence.Repositories.Buyer;

public class OrderItemRepository : EfRepositoryBase<OrderItem, Guid, BaseDbContext>, IOrderItemRepository
{
    public OrderItemRepository(BaseDbContext context) : base(context)
    {
    }
}
