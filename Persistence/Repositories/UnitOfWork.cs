using Microsoft.EntityFrameworkCore.Storage;
using Persistence.Contexts;
using Persistence.IRepositories;
using Persistence.IRepositories.Buyer;

namespace Persistence.Repositories;

public class UnitOfWork : IUnitOfWork, IAsyncDisposable
{
    private readonly BaseDbContext _context;
    private IDbContextTransaction? _transaction;

    public IBasketRepository BasketRepository { get; }
    public IBasketItemRepository BasketItemRepository { get; }
    public IBasketItemValueRepository BasketItemValueRepository { get; }
    public IBasketItemValueItemValueRepository BasketItemValueItemValueRepository { get; }

    public UnitOfWork(BaseDbContext context, IBasketRepository basketRepository, IBasketItemRepository basketItemRepository, IBasketItemValueRepository basketItemValueRepository, IBasketItemValueItemValueRepository basketItemValueItemValueRepository)
    {
        _context = context;
        BasketRepository = basketRepository;
        BasketItemRepository = basketItemRepository;
        BasketItemValueRepository = basketItemValueRepository;
        BasketItemValueItemValueRepository = basketItemValueItemValueRepository;
    }

    public async Task BeginTransactionAsync()
    {
        _transaction = await _context.Database.BeginTransactionAsync();
    }

    public async Task CommitTransactionAsync()
    {
        if (_transaction != null)
        {
            await _transaction.CommitAsync();
            await _transaction.DisposeAsync();
        }
    }

    public async Task RollbackTransactionAsync()
    {
        if (_transaction != null)
        {
            await _transaction.RollbackAsync();
            await _transaction.DisposeAsync();
        }
    }

    public async Task<int> CompleteAsync()
    {
        return await _context.SaveChangesAsync();
    }

    public void Dispose()
    {
        _context.Dispose();
        _transaction?.Dispose();
    }

    public async ValueTask DisposeAsync()
    {
        await _context.DisposeAsync();
        if (_transaction != null) await _transaction.DisposeAsync();
    }
}