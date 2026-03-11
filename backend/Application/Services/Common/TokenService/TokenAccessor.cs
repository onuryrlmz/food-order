using Base.Constant;
using Base.Entities;
using Microsoft.AspNetCore.Http;

namespace Application.Services.Common.TokenService;

public class TokenAccessor : ITokenAccessor
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public TokenAccessor(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public TokenDto? GetToken()
    {
        var httpContext = _httpContextAccessor.HttpContext;
        if (httpContext == null)
            return null;
        var client = new Client(httpContext);
        return client._tokenDto;
    }
}