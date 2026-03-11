using Base.Constant;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Helpers;

public class BaseController : ControllerBase
{
    protected string? getIpAddress()
    {
        if (Request.Headers.ContainsKey("X-Forwarded-For"))
            return Request.Headers["X-Forwarded-For"];
        return HttpContext.Connection.RemoteIpAddress?.MapToIPv4().ToString();
    }

    #region Client

    public Client? Client => _client ??= new Client(HttpContext);
    private Client? _client;

    #endregion
}