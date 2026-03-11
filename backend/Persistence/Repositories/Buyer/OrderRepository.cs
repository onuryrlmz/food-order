using Domain.Entities.Buyer;
using NArchitecture.Core.Persistence.Repositories;
using Persistence.Contexts;
using Persistence.IRepositories.Buyer;

namespace Persistence.Repositories.Buyer;

public class OrderRepository : EfRepositoryBase<Order, Guid, BaseDbContext>, IOrderRepository
{
    public OrderRepository(BaseDbContext context) : base(context)
    {
    }
}
