using Domain.Entities.Buyer;
using NArchitecture.Core.Persistence.Repositories;

namespace Persistence.IRepositories.Buyer;

public interface IBasketRepository : IAsyncRepository<Basket, Guid>, IRepository<Basket, Guid>
{
}