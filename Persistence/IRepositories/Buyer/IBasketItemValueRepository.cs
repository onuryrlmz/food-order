using Domain.Entities.Buyer;
using NArchitecture.Core.Persistence.Repositories;

namespace Persistence.IRepositories.Buyer;

public interface IBasketItemValueRepository : IAsyncRepository<BasketItemValue, Guid>, IRepository<BasketItemValue, Guid>
{
}