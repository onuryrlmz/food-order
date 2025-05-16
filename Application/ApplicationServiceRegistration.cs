using System.Reflection;
using System.Security.Authentication;
using Application.Services.Buyer;
using Application.Services.Buyer.BasketService;
using Application.Services.Common;
using Application.Services.Seller;
using Application.Services.Seller._0_CuisineService;
using Application.Services.Seller._1_SellerService;
using Application.Services.Seller._2_RestaurantService;
using Application.Services.Seller._3_ProductService;
using Application.Services.Seller._4_MenuService;
using Application.Services.Seller._5_MenuOptionService;
using Application.Services.Seller._6_MenuOptionValueService;
using Application.Services.Seller._7_MenuOptionValueOptionService;
using Application.Services.Seller._8_MenuOptionValueOptionValueService;
using Application.Services.Seller._99_RestaurantTransferService;
using Base.Constant;
using Base.Entities;
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
        services.AddAutoMapper(Assembly.GetExecutingAssembly());
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

        //Seller
        services.AddScoped<ISellerService, SellerManager>();
        services.AddScoped<IRestaurantService, RestaurantManager>();
        services.AddScoped<IRestaurantTransferService, RestaurantTransferService>();
        services.AddScoped<IProductService, ProductManager>();
        services.AddScoped<ICuisineService, CuisineManager>();
        services.AddScoped<IMenuService, MenuManager>();
        services.AddScoped<IMenuOptionService, MenuOptionManager>();
        services.AddScoped<IMenuOptionValueService, MenuOptionValueManager>();
        services.AddScoped<IMenuOptionValueOptionService, MenuOptionValueOptionManager>();
        services.AddScoped<IMenuOptionValueOptionValueService, MenuOptionValueOptionValueManager>();

        //Buyer
        services.AddScoped<IBasketService, BasketManager>();


        services.AddScoped<ITokenAccessor, TokenAccessor>();
        services.AddScoped<IRedisService, RedisManager>();

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