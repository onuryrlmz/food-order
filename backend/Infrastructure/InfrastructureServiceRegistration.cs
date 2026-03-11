using Infrastructure.Adapters.AwsS3;
using Infrastructure.Adapters.GetirAdapter;
using Infrastructure.Adapters.IyzicoServiceAdapter;
using Infrastructure.Adapters.YemekSepetiAdapter;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure;

public static class InfrastructureServiceRegistration
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services)
    {
        services.AddScoped<IAwsS3ServiceAdapter, AwsS3ServiceAdapter>();
        services.AddScoped<IIyzicoServiceAdapter, IyzicoServiceAdapter>();
        services.AddScoped<IGetirServiceAdapter, GetirServiceAdapter>();
        services.AddScoped<IGetirServiceAdapterV2, GetirServiceAdapterV2>();
        services.AddScoped<IYemekSepetiAdapter, YemekSepetiAdapter>();

        return services;
    }
}