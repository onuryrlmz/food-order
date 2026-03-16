using Infrastructure.Adapters.AwsS3;
using Infrastructure.Adapters.GetirAdapter;
using Infrastructure.Adapters.IyzicoServiceAdapter;
using Infrastructure.Adapters.OneSignalAdapter;
using Infrastructure.Adapters.YemekSepetiAdapter;
using Infrastructure.Sms;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure;

public static class InfrastructureServiceRegistration
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services)
    {
        services.AddScoped<IAwsS3ServiceAdapter, AwsS3ServiceAdapter>();
        services.AddScoped<IIyzicoServiceAdapter, IyzicoServiceAdapter>();
        services.AddScoped<IGetirServiceAdapter, GetirServiceAdapter>();
        services.AddScoped<IYemekSepetiAdapter, YemekSepetiAdapter>();
        services.AddScoped<ISmsSender, NetGsmSmsSender>();
        services.AddHttpClient<INotificationService, OneSignalNotificationService>();

        return services;
    }
}