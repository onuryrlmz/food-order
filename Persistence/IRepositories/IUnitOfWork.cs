using Persistence.IRepositories.Buyer;
using Persistence.IRepositories.Seller;

namespace Persistence.IRepositories;

public interface IUnitOfWork : IDisposable
{
    IBasketRepository BasketRepository { get; }
    IBasketItemRepository BasketItemRepository { get; }
    IBasketItemValueRepository BasketItemValueRepository { get; }
    IBasketItemValueItemValueRepository BasketItemValueItemValueRepository { get; }
    Task<int> CompleteAsync();
    Task BeginTransactionAsync();
    Task CommitTransactionAsync();
    Task RollbackTransactionAsync();
}