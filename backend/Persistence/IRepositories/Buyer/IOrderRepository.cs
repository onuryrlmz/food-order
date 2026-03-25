using Domain.Entities.Buyer;
using NArchitecture.Core.Persistence.Repositories;
using Persistence.Contexts;

namespace Persistence.IRepositories.Buyer;

public interface IOrderRepository : IAsyncRepository<Order, Guid>, IRepository<Order, Guid>
{
}