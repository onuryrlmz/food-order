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
using Persistence.Repositories.Seller;

namespace Persistence;

public static class PersistenceServiceRegistration
{
    public static IServiceCollection AddPersistenceServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<BaseDbContext>(options => options.UseMySql(configuration.GetConnectionString("FoodOrderApp"), new MySqlServerVersion(new Version(8, 4, 4)))
        );


        services.AddScoped<IUnitOfWork, UnitOfWork>();

        //Common
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IAddressRepository, AddressRepository>();
        services.AddScoped<IScheduledTaskRepository, ScheduledTaskRepository>();

        //Seller
        services.AddScoped<ISellerRepository, SellerRepository>();
        services.AddScoped<ISellerDetailRepository, SellerDetailRepository>();
        services.AddScoped<IRestaurantRepository, RestaurantRepository>();
        services.AddScoped<ICategoryRepository, CategoryRepository>();
        services.AddScoped<ICategoryDetailRepository, CategoryDetailRepository>();
        services.AddScoped<ICuisineRepository, CuisineRepository>();
        services.AddScoped<IProductRepository, ProductRepository>();
        services.AddScoped<IMenuRepository, MenuRepository>();
        services.AddScoped<IMenuOptionRepository, MenuOptionRepository>();
        services.AddScoped<IMenuOptionValueRepository, MenuOptionValueRepository>();
        services.AddScoped<IMenuOptionValueOptionRepository, MenuOptionValueOptionRepository>();
        services.AddScoped<IMenuOptionValueOptionValueRepository, MenuOptionValueOptionValueRepository>();

        //Buyer
        services.AddScoped<IBasketRepository, BasketRepository>();
        services.AddScoped<IBasketItemRepository, BasketItemRepository>();
        services.AddScoped<IBasketItemValueRepository, BasketItemValueRepository>();
        services.AddScoped<IBasketItemValueItemValueRepository, BasketItemValueItemValueRepository>();

        return services;
    }
}