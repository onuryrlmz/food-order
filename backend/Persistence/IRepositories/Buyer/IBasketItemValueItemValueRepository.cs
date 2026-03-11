using Domain.Entities.Buyer;
using NArchitecture.Core.Persistence.Repositories;

namespace Persistence.IRepositories.Buyer;

public interface IBasketItemValueItemValueRepository : IAsyncRepository<BasketItemValueItemValue, Guid>, IRepository<BasketItemValueItemValue, Guid>
{
}