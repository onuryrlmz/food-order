using Domain.Entities.Buyer;
using NArchitecture.Core.Persistence.Repositories;

namespace Persistence.IRepositories.Buyer;

public interface IBasketItemRepository : IAsyncRepository<BasketItem, Guid>, IRepository<BasketItem, Guid>
{
}