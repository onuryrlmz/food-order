using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Persistence.Contexts;
using Persistence.IRepositories;
using Persistence.IRepositories.Buyer;
using Persistence.IRepositories.Common;
using Persistence.IRepositories.Seller;
using Persistence.Repositories;
using Persistence.Repositories.Buyer;
using Persistence.Repositories.Common;
using Persistence.Repositories.Courier;
using Persistence.Repositories.Seller;
using Persistence.IRepositories.Courier;

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

        //Seller
        services.AddScoped<ICuisineRepository, CuisineRepository>();
        services.AddScoped<ISellerRepository, SellerRepository>();
        services.AddScoped<ISellerDetailRepository, SellerDetailRepository>();
        services.AddScoped<IProductRepository, ProductRepository>();
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
        services.AddScoped<IRestaurantCdnUpdateQueueRepository, RestaurantCdnUpdateQueueRepository>();
        services.AddScoped<IRestaurantWorkingHourRepository, RestaurantWorkingHourRepository>();

        //Courier
        services.AddScoped<IRestaurantCourierRepository, RestaurantCourierRepository>();
        services.AddScoped<ICourierLocationRepository, CourierLocationRepository>();
        services.AddScoped<ICourierCompanyRepository, CourierCompanyRepository>();
        services.AddScoped<ICourierCompanyMemberRepository, CourierCompanyMemberRepository>();
        services.AddScoped<IRestaurantCourierCompanyRepository, RestaurantCourierCompanyRepository>();
        services.AddScoped<ISubscriptionUsageRepository, SubscriptionUsageRepository>();

        return services;
    }
}