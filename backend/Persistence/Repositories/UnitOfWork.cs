using Microsoft.EntityFrameworkCore.Storage;
using Persistence.IRepositories.Seller;
using Persistence.Contexts;
using Persistence.IRepositories;
using Persistence.IRepositories.Buyer;
using Persistence.IRepositories.Common;
using Persistence.IRepositories.Courier;
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
    public IBasketRepository BasketRepository { get; }
    public IBasketItemRepository BasketItemRepository { get; }
    public IBasketItemValueRepository BasketItemValueRepository { get; }
    public IBasketItemValueItemValueRepository BasketItemValueItemValueRepository { get; }
    public IOrderRepository OrderRepository { get; }
    public IOrderItemRepository OrderItemRepository { get; }
    public IPaymentRepository PaymentRepository { get; }
    public IOrderStatusHistoryRepository OrderStatusHistoryRepository { get; }
    public IOptionTemplateRepository OptionTemplateRepository { get; }
    public IOptionTemplateValueRepository OptionTemplateValueRepository { get; }
    public IOptionTemplateValueOptionRepository OptionTemplateValueOptionRepository { get; }
    public IOptionTemplateValueOptionValueRepository OptionTemplateValueOptionValueRepository { get; }
    public IRestaurantCdnUpdateQueueRepository RestaurantCdnUpdateQueueRepository { get; }
    public IRestaurantWorkingHourRepository RestaurantWorkingHourRepository { get; }

    // Buyer - Reviews & Favorites
    public IReviewRepository ReviewRepository { get; }
    public IFavoriteRestaurantRepository FavoriteRestaurantRepository { get; }

    // Auth
    public IRefreshTokenRepository RefreshTokenRepository { get; }
    public IPasswordResetTokenRepository PasswordResetTokenRepository { get; }

    // Courier
    public ICourierCompanyRepository CourierCompanyRepository { get; }
    public ICourierRepository CourierRepository { get; }
    public IRestaurantCourierAgreementRepository RestaurantCourierAgreementRepository { get; }
    public IDeliveryAssignmentRepository DeliveryAssignmentRepository { get; }
    public ICourierEarningRepository CourierEarningRepository { get; }
    public ICourierLocationHistoryRepository CourierLocationHistoryRepository { get; }

    // Buyer - Tip & Search
    public ITipRepository TipRepository { get; }
    public ISearchHistoryRepository SearchHistoryRepository { get; }

    // Buyer - Notification
    public INotificationRepository NotificationRepository { get; }
    public INotificationPreferenceRepository NotificationPreferenceRepository { get; }

    // Buyer - Scheduled Order
    public IScheduledOrderRepository ScheduledOrderRepository { get; }

    // Buyer - AI Support
    public ISupportTicketRepository SupportTicketRepository { get; }
    public ISupportMessageRepository SupportMessageRepository { get; }
    public ISupportActionRepository SupportActionRepository { get; }

    // Commission & Settlement
    public IPlatformCommissionScheduleRepository PlatformCommissionScheduleRepository { get; }
    public IRestaurantCommissionRepository RestaurantCommissionRepository { get; }
    public ISettlementItemRepository SettlementItemRepository { get; }
    public ISettlementPeriodRepository SettlementPeriodRepository { get; }

    // Buyer - Payment Log
    public IPaymentLogRepository PaymentLogRepository { get; }

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
        IBasketRepository basketRepository,
        IBasketItemRepository basketItemRepository,
        IBasketItemValueRepository basketItemValueRepository,
        IBasketItemValueItemValueRepository basketItemValueItemValueRepository,
        IOrderRepository orderRepository,
        IOrderItemRepository orderItemRepository,
        IPaymentRepository paymentRepository,
        IOrderStatusHistoryRepository orderStatusHistoryRepository,
        IOptionTemplateRepository optionTemplateRepository,
        IOptionTemplateValueRepository optionTemplateValueRepository,
        IOptionTemplateValueOptionRepository optionTemplateValueOptionRepository,
        IOptionTemplateValueOptionValueRepository optionTemplateValueOptionValueRepository,
        IRestaurantCdnUpdateQueueRepository restaurantCdnUpdateQueueRepository,
        IRestaurantWorkingHourRepository restaurantWorkingHourRepository,
        IReviewRepository reviewRepository,
        IFavoriteRestaurantRepository favoriteRestaurantRepository,
        IRefreshTokenRepository refreshTokenRepository,
        IPasswordResetTokenRepository passwordResetTokenRepository,
        ICourierCompanyRepository courierCompanyRepository,
        ICourierRepository courierRepository,
        IRestaurantCourierAgreementRepository restaurantCourierAgreementRepository,
        IDeliveryAssignmentRepository deliveryAssignmentRepository,
        ICourierEarningRepository courierEarningRepository,
        ICourierLocationHistoryRepository courierLocationHistoryRepository,
        ITipRepository tipRepository,
        ISearchHistoryRepository searchHistoryRepository,
        INotificationRepository notificationRepository,
        INotificationPreferenceRepository notificationPreferenceRepository,
        IScheduledOrderRepository scheduledOrderRepository,
        ISupportTicketRepository supportTicketRepository,
        ISupportMessageRepository supportMessageRepository,
        ISupportActionRepository supportActionRepository,
        IPlatformCommissionScheduleRepository platformCommissionScheduleRepository,
        IRestaurantCommissionRepository restaurantCommissionRepository,
        ISettlementItemRepository settlementItemRepository,
        ISettlementPeriodRepository settlementPeriodRepository,
        IPaymentLogRepository paymentLogRepository)
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
        BasketRepository = basketRepository;
        BasketItemRepository = basketItemRepository;
        BasketItemValueRepository = basketItemValueRepository;
        BasketItemValueItemValueRepository = basketItemValueItemValueRepository;
        OrderRepository = orderRepository;
        OrderItemRepository = orderItemRepository;
        PaymentRepository = paymentRepository;
        OrderStatusHistoryRepository = orderStatusHistoryRepository;
        OptionTemplateRepository = optionTemplateRepository;
        OptionTemplateValueRepository = optionTemplateValueRepository;
        OptionTemplateValueOptionRepository = optionTemplateValueOptionRepository;
        OptionTemplateValueOptionValueRepository = optionTemplateValueOptionValueRepository;
        RestaurantCdnUpdateQueueRepository = restaurantCdnUpdateQueueRepository;
        RestaurantWorkingHourRepository = restaurantWorkingHourRepository;
        ReviewRepository = reviewRepository;
        FavoriteRestaurantRepository = favoriteRestaurantRepository;
        RefreshTokenRepository = refreshTokenRepository;
        PasswordResetTokenRepository = passwordResetTokenRepository;
        CourierCompanyRepository = courierCompanyRepository;
        CourierRepository = courierRepository;
        RestaurantCourierAgreementRepository = restaurantCourierAgreementRepository;
        DeliveryAssignmentRepository = deliveryAssignmentRepository;
        CourierEarningRepository = courierEarningRepository;
        CourierLocationHistoryRepository = courierLocationHistoryRepository;
        TipRepository = tipRepository;
        SearchHistoryRepository = searchHistoryRepository;
        NotificationRepository = notificationRepository;
        NotificationPreferenceRepository = notificationPreferenceRepository;
        ScheduledOrderRepository = scheduledOrderRepository;
        SupportTicketRepository = supportTicketRepository;
        SupportMessageRepository = supportMessageRepository;
        SupportActionRepository = supportActionRepository;
        PlatformCommissionScheduleRepository = platformCommissionScheduleRepository;
        RestaurantCommissionRepository = restaurantCommissionRepository;
        SettlementItemRepository = settlementItemRepository;
        SettlementPeriodRepository = settlementPeriodRepository;
        PaymentLogRepository = paymentLogRepository;
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