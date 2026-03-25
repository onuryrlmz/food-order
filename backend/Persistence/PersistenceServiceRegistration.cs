using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Persistence.Contexts;
using Persistence.IRepositories;
using Persistence.IRepositories.Buyer;
using Persistence.IRepositories.Common;
using Persistence.IRepositories.Courier;
using Persistence.IRepositories.Seller;
using Persistence.Repositories;
using Persistence.Repositories.Buyer;
using Persistence.Repositories.Common;
using Persistence.Repositories.Courier;
using Persistence.Repositories.Seller;

namespace Persistence;

public static class PersistenceServiceRegistration
{
    public static IServiceCollection AddPersistenceServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<BaseDbContext>(options => options.UseMySql(configuration.GetConnectionString("FoodOrderApp"), new MySqlServerVersion(new Version(8, 4, 4)))
        );

        //System
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        //Common
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IAddressRepository, AddressRepository>();
        services.AddScoped<IScheduledTaskRepository, ScheduledTaskRepository>();
        services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();
        services.AddScoped<IPasswordResetTokenRepository, PasswordResetTokenRepository>();

        //Seller
        services.AddScoped<ICuisineRepository, CuisineRepository>();
        services.AddScoped<ISellerRepository, SellerRepository>();
        services.AddScoped<ISellerDetailRepository, SellerDetailRepository>();
        services.AddScoped<IProductRepository, ProductRepository>();
        services.AddScoped<IProductImageRepository, ProductImageRepository>();
        services.AddScoped<IProductAttributeRepository, ProductAttributeRepository>();
        services.AddScoped<IProductAttributeValueRepository, ProductAttributeValueRepository>();
        services.AddScoped<IRestaurantRepository, RestaurantRepository>();
        services.AddScoped<ICategoryRepository, CategoryRepository>();
        services.AddScoped<ICategoryDetailRepository, CategoryDetailRepository>();
        services.AddScoped<IMenuRepository, MenuRepository>();
        services.AddScoped<IMenuOptionRepository, MenuOptionRepository>();
        services.AddScoped<IMenuOptionValueRepository, MenuOptionValueRepository>();
        services.AddScoped<IMenuOptionValueOptionRepository, MenuOptionValueOptionRepository>();
        services.AddScoped<IMenuOptionValueOptionValueRepository, MenuOptionValueOptionValueRepository>();
        services.AddScoped<ISubscriptionPlanRepository, SubscriptionPlanRepository>();
        services.AddScoped<ISubscriptionRepository, SubscriptionRepository>();
        services.AddScoped<IOptionTemplateRepository, OptionTemplateRepository>();
        services.AddScoped<IOptionTemplateValueRepository, OptionTemplateValueRepository>();
        services.AddScoped<IOptionTemplateValueOptionRepository, OptionTemplateValueOptionRepository>();
        services.AddScoped<IOptionTemplateValueOptionValueRepository, OptionTemplateValueOptionValueRepository>();
        services.AddScoped<ICouponRepository, CouponRepository>();

        //Buyer
        services.AddScoped<IBasketRepository, BasketRepository>();
        services.AddScoped<IBasketItemRepository, BasketItemRepository>();
        services.AddScoped<IBasketItemValueRepository, BasketItemValueRepository>();
        services.AddScoped<IBasketItemValueItemValueRepository, BasketItemValueItemValueRepository>();
        services.AddScoped<IOrderRepository, OrderRepository>();
        services.AddScoped<IOrderItemRepository, OrderItemRepository>();
        services.AddScoped<IPaymentRepository, PaymentRepository>();
        services.AddScoped<IOrderStatusHistoryRepository, OrderStatusHistoryRepository>();
        services.AddScoped<IReviewRepository, ReviewRepository>();
        services.AddScoped<IFavoriteRestaurantRepository, FavoriteRestaurantRepository>();
        services.AddScoped<IRestaurantCdnUpdateQueueRepository, RestaurantCdnUpdateQueueRepository>();
        services.AddScoped<IRestaurantWorkingHourRepository, RestaurantWorkingHourRepository>();

        services.AddScoped<ISubscriptionUsageRepository, SubscriptionUsageRepository>();

        //Courier
        services.AddScoped<ICourierCompanyRepository, CourierCompanyRepository>();
        services.AddScoped<ICourierRepository, CourierRepository>();
        services.AddScoped<IRestaurantCourierAgreementRepository, RestaurantCourierAgreementRepository>();
        services.AddScoped<IDeliveryAssignmentRepository, DeliveryAssignmentRepository>();
        services.AddScoped<ICourierEarningRepository, CourierEarningRepository>();

        return services;
    }
}