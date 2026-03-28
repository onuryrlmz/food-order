using System.Reflection;
using System.Security.Authentication;
using Application.Services.Buyer.BasketService;
using Application.Services.Buyer.OrderService;
using Application.Services.Common.AddressService;
using Application.Services.Common.AuthService;
using Application.Services.Common.PasswordResetService;
using Application.Services.Common.RedisService;
using Application.Services.Common.TokenService;
using Application.Services.Common.UserService;
using Application.Services.Seller.CuisineService;
using Application.Services.Seller.SellerService;
using Application.Services.Seller.CategoryDetailService;
using Application.Services.Seller.RestaurantService;
using Application.Services.Seller.ProductService;
using Application.Services.Seller.ProductAttributeService;
using Application.Services.Seller.ProductAttributeValueService;
using Application.Services.Seller.MenuOptionValueService;
using Application.Services.Seller.MenuService;
using Application.Services.Seller.MenuOptionService;
using Application.Services.Seller.MenuOptionValueOptionService;
using Application.Services.Seller.MenuOptionValueOptionValueService;
using Application.Services.Seller.CategoryService;
using Application.Services.Seller.RestaurantTransferService;
using Application.Services.Seller.CdnWorkerService;
using Application.Services.Seller.CouponService;
using Application.Services.Seller.ImageUploadService;
using Application.Services.Seller.OptionTemplateService;
using Application.Services.Buyer.CardService;
using Application.Services.Buyer.CouponService;
using Application.Services.Buyer.PaymentService;
using Application.Services.Buyer.TipService;
using Application.Services.Buyer.SearchHistoryService;
using Application.Services.Buyer.ProfileService;
using Application.Services.Buyer.NotificationService;
using Application.Services.Buyer.AiSupportService;
using Application.Services.Buyer.ScheduledOrderService;
using Application.Services.Buyer.FavoriteService;
using Application.Services.Buyer.ReviewService;
using Application.Services.Seller.CommissionService;
using Application.Services.Analytics;
using Application.Services.Common.BackgroundJobs;
using Application.Services.Courier.CourierService;
using Application.Services.Courier.CourierCompanyService;
using Application.Services.Courier.DeliveryAssignmentService;
using Application.Services.Courier.CourierEarningService;
using Base.Constant;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using NArchitecture.Core.Application.Pipelines.Authorization;
using NArchitecture.Core.Application.Pipelines.Caching;
using NArchitecture.Core.Application.Pipelines.Logging;
using NArchitecture.Core.Application.Pipelines.Transaction;
using NArchitecture.Core.Application.Pipelines.Validation;
using NArchitecture.Core.Application.Rules;
using StackExchange.Redis;

namespace Application;

public static class ApplicationServiceRegistration
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddAutoMapper(cfg => cfg.AddMaps(Assembly.GetExecutingAssembly()));
        services.AddMediatR(configuration =>
        {
            configuration.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());
            configuration.AddOpenBehavior(typeof(AuthorizationBehavior<,>));
            configuration.AddOpenBehavior(typeof(CachingBehavior<,>));
            configuration.AddOpenBehavior(typeof(CacheRemovingBehavior<,>));
            configuration.AddOpenBehavior(typeof(LoggingBehavior<,>));
            configuration.AddOpenBehavior(typeof(RequestValidationBehavior<,>));
            configuration.AddOpenBehavior(typeof(TransactionScopeBehavior<,>));
        });

        services.AddSingleton<IConnectionMultiplexer>(sp =>
        {
            var config = Global.Configuration.GetSection("Redis");
            var options = new ConfigurationOptions
            {
                EndPoints = { $"{config["Host"]}:{config["Port"]}" },
                Password = config["Password"],
                Ssl = bool.Parse(config["Ssl"] ?? "false"),
                SslProtocols = SslProtocols.Tls12,
                AbortOnConnectFail = false,
                ConnectTimeout = 5000,
                SyncTimeout = 5000
            };

            return ConnectionMultiplexer.Connect(options);
        });

        services.AddSubClassesOfType(Assembly.GetExecutingAssembly(), typeof(BaseBusinessRules));
        services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
        //services.AddSingleton<IMailService, MailKitMailService>();
        //services.AddSingleton<ILogger, SerilogLoggerServiceBase>();
        //services.AddSingleton<IElasticSearch, ElasticSearchManager>();

        //Common
        services.AddScoped<IUserService, UserManager>();
        services.AddScoped<IAddressService, AddressManager>();
        services.AddScoped<ICuisineService, CuisineManager>();
        services.AddScoped<IAuthTokenService, AuthTokenManager>();
        services.AddScoped<IPasswordResetService, PasswordResetManager>();

        //Seller
        services.AddScoped<ISellerService, SellerManager>();
        services.AddScoped<IRestaurantService, RestaurantManager>();
        services.AddScoped<IRestaurantTransferService, RestaurantTransferService>();
        services.AddScoped<IProductService, ProductManager>();
        services.AddScoped<IProductAttributeService, ProductAttributeManager>();
        services.AddScoped<IProductAttributeValueService, ProductAttributeValueManager>();

        services.AddScoped<IMenuService, MenuManager>();
        services.AddScoped<IMenuOptionService, MenuOptionManager>();
        services.AddScoped<IMenuOptionValueService, MenuOptionValueManager>();
        services.AddScoped<IMenuOptionValueOptionService, MenuOptionValueOptionManager>();
        services.AddScoped<IMenuOptionValueOptionValueService, MenuOptionValueOptionValueManager>();
        services.AddScoped<ICategoryService, CategoryService>();
        services.AddScoped<ICategoryDetailService, CategoryDetailService>();
        services.AddScoped<IOptionTemplateService, OptionTemplateManager>();
        services.AddScoped<IImageUploadService, ImageUploadManager>();

        //Buyer
        services.AddScoped<IBasketService, BasketManager>();
        services.AddScoped<IOrderService, OrderManager>();
        services.AddScoped<IReviewService, ReviewManager>();
        services.AddScoped<IFavoriteService, FavoriteManager>();
        services.AddScoped<ICardService, CardManager>();
        services.AddScoped<IPaymentService, PaymentManager>();
        services.AddScoped<ITipService, TipManager>();
        services.AddScoped<ISearchHistoryService, SearchHistoryManager>();
        services.AddScoped<IProfileService, ProfileManager>();
        services.AddScoped<INotificationSettingsService, NotificationSettingsManager>();
        services.AddScoped<IScheduledOrderService, ScheduledOrderManager>();
        services.AddScoped<IAiSupportService, AiSupportManager>();

        //Commission & Settlement
        services.AddScoped<ICommissionService, CommissionManager>();

        //Coupon
        services.AddScoped<ICouponService, CouponManager>();
        services.AddScoped<ICouponValidationService, CouponValidationManager>();

        //Analytics
        services.AddScoped<IAnalyticsService, AnalyticsManager>();

        //Global
        services.AddScoped<ITokenAccessor, TokenAccessor>();
        services.AddScoped<IRedisService, RedisManager>();

        //Background Workers
        services.AddHostedService<RestaurantCdnWorker>();

        //Background Jobs (Hangfire)
        services.AddScoped<ICleanupJobService, CleanupJobService>();
        services.AddScoped<IDeliveryTimeoutJobService, DeliveryTimeoutJobService>();
        services.AddScoped<ICourierJobService, CourierJobService>();
        services.AddScoped<IScheduledOrderJobService, ScheduledOrderJobService>();
        services.AddScoped<ISettlementJobService, SettlementJobService>();

        //Courier
        services.AddScoped<ICourierService, CourierManager>();
        services.AddScoped<ICourierCompanyService, CourierCompanyManager>();
        services.AddScoped<IDeliveryAssignmentService, DeliveryAssignmentManager>();
        services.AddScoped<ICourierEarningService, CourierEarningManager>();

        return services;
    }

    private static IServiceCollection AddSubClassesOfType(this IServiceCollection services, Assembly assembly, Type type, Func<IServiceCollection, Type, IServiceCollection>? addWithLifeCycle = null)
    {
        var types = assembly.GetTypes().Where(t => t.IsSubclassOf(type) && type != t).ToList();
        foreach (var item in types)
            if (addWithLifeCycle == null)
                services.AddScoped(item);
            else
                addWithLifeCycle(services, type);
        return services;
    }
}