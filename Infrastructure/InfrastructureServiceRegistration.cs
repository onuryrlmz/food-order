using Infrastructure.Adapters.GetirAdapter;
using Infrastructure.Adapters.IyzicoServiceAdapter;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure;

public static class InfrastructureServiceRegistration
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services)
    {
        services.AddScoped<IIyzicoServiceAdapter, IyzicoServiceAdapter>();
        services.AddScoped<IGetirServiceAdapter, GetirServiceAdapter>();

        return services;
    }
}