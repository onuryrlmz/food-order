using Microsoft.EntityFrameworkCore.Storage;
using Persistence.IRepositories.Seller;
using Persistence.Contexts;
using Persistence.IRepositories;
using Persistence.IRepositories.Buyer;
using Persistence.IRepositories.Common;
using Persistence.IRepositories.Seller;

namespace Persistence.Repositories;

public class UnitOfWork : IUnitOfWork, IAsyncDisposable
{
    private readonly BaseDbContext _context;
    private IDbContextTransaction? _transaction;

    public ICuisineRepository CuisineRepository { get; set; }
    public ISellerRepository SellerRepository { get; set; }
    public IRestaurantRepository RestaurantRepository { get; set; }
    public IProductRepository ProductRepository { get; }
    public ICategoryRepository CategoryRepository { get; set; }
    public ICategoryDetailRepository CategoryDetailRepository { get; set; }
    public IMenuRepository MenuRepository { get; set; }
    public IMenuOptionRepository MenuOptionRepository { get; }
    public IMenuOptionValueRepository MenuOptionValueRepository { get; set; }
    public IMenuOptionValueOptionRepository MenuOptionValueOptionRepository { get; set; }
    public IMenuOptionValueOptionValueRepository MenuOptionValueOptionValueRepository { get; set; }
    public ISubscriptionPlanRepository SubscriptionPlanRepository { get; set; }
    public ISubscriptionRepository SubscriptionRepository { get; set; }
    public IBasketRepository BasketRepository { get; }
    public IBasketItemRepository BasketItemRepository { get; }
    public IBasketItemValueRepository BasketItemValueRepository { get; }
    public IBasketItemValueItemValueRepository BasketItemValueItemValueRepository { get; }
    public IOrderRepository OrderRepository { get; }
    public IOrderItemRepository OrderItemRepository { get; }
    public IOptionTemplateRepository OptionTemplateRepository { get; }
    public IOptionTemplateValueRepository OptionTemplateValueRepository { get; }
    public IOptionTemplateValueOptionRepository OptionTemplateValueOptionRepository { get; }
    public IOptionTemplateValueOptionValueRepository OptionTemplateValueOptionValueRepository { get; }
    public IRestaurantCdnUpdateQueueRepository RestaurantCdnUpdateQueueRepository { get; }
    public IRestaurantWorkingHourRepository RestaurantWorkingHourRepository { get; }

    public UnitOfWork(BaseDbContext context,
        ICuisineRepository cuisineRepository,
        ISellerRepository sellerRepository,
        IRestaurantRepository restaurantRepository,
        IProductRepository productRepository,
        ICategoryRepository categoryRepository,
        ICategoryDetailRepository categoryDetailRepository,
        IMenuRepository menuRepository,
        IMenuOptionRepository menuOptionRepository,
        IMenuOptionValueRepository menuOptionValueRepository,
        IMenuOptionValueOptionRepository menuOptionValueOptionRepository,
        IMenuOptionValueOptionValueRepository menuOptionValueOptionValueRepository,
        ISubscriptionPlanRepository subscriptionPlanRepository,
        ISubscriptionRepository subscriptionRepository,
        IBasketRepository basketRepository,
        IBasketItemRepository basketItemRepository,
        IBasketItemValueRepository basketItemValueRepository,
        IBasketItemValueItemValueRepository basketItemValueItemValueRepository,
        IOrderRepository orderRepository,
        IOrderItemRepository orderItemRepository,
        IOptionTemplateRepository optionTemplateRepository,
        IOptionTemplateValueRepository optionTemplateValueRepository,
        IOptionTemplateValueOptionRepository optionTemplateValueOptionRepository,
        IOptionTemplateValueOptionValueRepository optionTemplateValueOptionValueRepository,
        IRestaurantCdnUpdateQueueRepository restaurantCdnUpdateQueueRepository,
        IRestaurantWorkingHourRepository restaurantWorkingHourRepository)
    {
        _context = context;
        CuisineRepository = cuisineRepository;
        SellerRepository = sellerRepository;
        RestaurantRepository = restaurantRepository;
        ProductRepository = productRepository;
        CategoryRepository = categoryRepository;
        CategoryDetailRepository = categoryDetailRepository;
        MenuRepository = menuRepository;
        MenuOptionRepository = menuOptionRepository;
        MenuOptionValueRepository = menuOptionValueRepository;
        MenuOptionValueOptionRepository = menuOptionValueOptionRepository;
        MenuOptionValueOptionValueRepository = menuOptionValueOptionValueRepository;
        SubscriptionPlanRepository = subscriptionPlanRepository;
        SubscriptionRepository = subscriptionRepository;
        BasketRepository = basketRepository;
        BasketItemRepository = basketItemRepository;
        BasketItemValueRepository = basketItemValueRepository;
        BasketItemValueItemValueRepository = basketItemValueItemValueRepository;
        OrderRepository = orderRepository;
        OrderItemRepository = orderItemRepository;
        OptionTemplateRepository = optionTemplateRepository;
        OptionTemplateValueRepository = optionTemplateValueRepository;
        OptionTemplateValueOptionRepository = optionTemplateValueOptionRepository;
        OptionTemplateValueOptionValueRepository = optionTemplateValueOptionValueRepository;
        RestaurantCdnUpdateQueueRepository = restaurantCdnUpdateQueueRepository;
        RestaurantWorkingHourRepository = restaurantWorkingHourRepository;
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
