using Persistence.IRepositories.Buyer;
using Persistence.IRepositories.Common;
using Persistence.IRepositories.Courier;
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
    ISubscriptionPlanRepository SubscriptionPlanRepository { get; set; }
    ISubscriptionRepository SubscriptionRepository { get; set; }
    IBasketRepository BasketRepository { get; }
    IBasketItemRepository BasketItemRepository { get; }
    IBasketItemValueRepository BasketItemValueRepository { get; }
    IBasketItemValueItemValueRepository BasketItemValueItemValueRepository { get; }
    IOrderRepository OrderRepository { get; }
    IOrderItemRepository OrderItemRepository { get; }
    IPaymentRepository PaymentRepository { get; }
    IOrderStatusHistoryRepository OrderStatusHistoryRepository { get; }
    IOptionTemplateRepository OptionTemplateRepository { get; }
    IOptionTemplateValueRepository OptionTemplateValueRepository { get; }
    IOptionTemplateValueOptionRepository OptionTemplateValueOptionRepository { get; }
    IOptionTemplateValueOptionValueRepository OptionTemplateValueOptionValueRepository { get; }
    IRestaurantCdnUpdateQueueRepository RestaurantCdnUpdateQueueRepository { get; }
    IRestaurantWorkingHourRepository RestaurantWorkingHourRepository { get; }

    // Courier
    IRestaurantCourierRepository RestaurantCourierRepository { get; }
    ICourierLocationRepository CourierLocationRepository { get; }
    ICourierCompanyRepository CourierCompanyRepository { get; }
    ICourierCompanyMemberRepository CourierCompanyMemberRepository { get; }
    IRestaurantCourierCompanyRepository RestaurantCourierCompanyRepository { get; }

    // Subscription Usage
    ISubscriptionUsageRepository SubscriptionUsageRepository { get; }

    // Auth
    IRefreshTokenRepository RefreshTokenRepository { get; }
    IPasswordResetTokenRepository PasswordResetTokenRepository { get; }

    Task<int> CompleteAsync();
    Task BeginTransactionAsync();
    Task CommitTransactionAsync();
    Task RollbackTransactionAsync();
}