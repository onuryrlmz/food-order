using Persistence.IRepositories.Buyer;
using Persistence.IRepositories.Common;
using Persistence.IRepositories.Seller;

namespace Persistence.IRepositories;

public interface IUnitOfWork : IDisposable
{
    ICuisineRepository CuisineRepository { get; set; }
    ISellerRepository SellerRepository { get; set; }
    IRestaurantRepository RestaurantRepository { get; set; }
    IProductRepository ProductRepository { get; }
    ICategoryRepository CategoryRepository { get; set; }
    ICategoryDetailRepository CategoryDetailRepository { get; set; }
    IMenuRepository MenuRepository { get; set; }
    IMenuOptionRepository MenuOptionRepository { get; }
    IMenuOptionValueRepository MenuOptionValueRepository { get; set; }
    IMenuOptionValueOptionRepository MenuOptionValueOptionRepository { get; set; }
    IMenuOptionValueOptionValueRepository MenuOptionValueOptionValueRepository { get; set; }
    IBasketRepository BasketRepository { get; }
    IBasketItemRepository BasketItemRepository { get; }
    IBasketItemValueRepository BasketItemValueRepository { get; }
    IBasketItemValueItemValueRepository BasketItemValueItemValueRepository { get; }

    Task<int> CompleteAsync();
    Task BeginTransactionAsync();
    Task CommitTransactionAsync();
    Task RollbackTransactionAsync();
}