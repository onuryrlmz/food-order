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

    // Subscription Usage
    ISubscriptionUsageRepository SubscriptionUsageRepository { get; }

    // Buyer - Reviews & Favorites
    IReviewRepository ReviewRepository { get; }
    IFavoriteRestaurantRepository FavoriteRestaurantRepository { get; }

    // Auth
    IRefreshTokenRepository RefreshTokenRepository { get; }
    IPasswordResetTokenRepository PasswordResetTokenRepository { get; }

    // Courier
    ICourierCompanyRepository CourierCompanyRepository { get; }
    ICourierRepository CourierRepository { get; }
    IRestaurantCourierAgreementRepository RestaurantCourierAgreementRepository { get; }
    IDeliveryAssignmentRepository DeliveryAssignmentRepository { get; }
    ICourierEarningRepository CourierEarningRepository { get; }

    // Buyer - Tip & Search
    ITipRepository TipRepository { get; }
    ISearchHistoryRepository SearchHistoryRepository { get; }

    // Buyer - Notification
    INotificationRepository NotificationRepository { get; }
    INotificationPreferenceRepository NotificationPreferenceRepository { get; }

    // Buyer - Scheduled Order
    IScheduledOrderRepository ScheduledOrderRepository { get; }

    // Buyer - AI Support
    ISupportTicketRepository SupportTicketRepository { get; }
    ISupportMessageRepository SupportMessageRepository { get; }
    ISupportActionRepository SupportActionRepository { get; }

    Task<int> CompleteAsync();
    Task BeginTransactionAsync();
    Task CommitTransactionAsync();
    Task RollbackTransactionAsync();
}