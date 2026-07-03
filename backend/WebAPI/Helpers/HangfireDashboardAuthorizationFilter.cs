using Hangfire.Annotations;
using Hangfire.Dashboard;

namespace WebAPI.Helpers;

/// <summary>
/// Hangfire panosuna erişimi kısıtlar. Geliştirme ortamında serbest; aksi halde yalnızca
/// yerel (loopback) isteklere izin verir. Böylece pano yetkilendirme olmadan internete açılmaz.
/// </summary>
public class HangfireDashboardAuthorizationFilter : IDashboardAuthorizationFilter
{
    private readonly bool _isDevelopment;

    public HangfireDashboardAuthorizationFilter(bool isDevelopment)
    {
        _isDevelopment = isDevelopment;
    }

    public bool Authorize([NotNull] DashboardContext context)
    {
        if (_isDevelopment)
            return true;

        var httpContext = context.GetHttpContext();
        var remoteIp = httpContext.Connection.RemoteIpAddress;
        return remoteIp != null && System.Net.IPAddress.IsLoopback(remoteIp);
    }
}
