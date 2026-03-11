using Base.Entities;
using Base.Security;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;

namespace Base.Constant;

public class Client : IDisposable
{
    private readonly HttpContext _httpContext;
    public Guid? _anonymousId;
    public TokenDto? _tokenDto;

    public Client(HttpContext httpContext)
    {
        _httpContext = httpContext;
        ParseRequestHeaders();
    }

    public void Dispose()
    {
        GC.SuppressFinalize(this);
    }

    private void ParseRequestHeaders()
    {
        try
        {
            var headers = _httpContext.Request.Headers;
            foreach (var header in headers)
                try
                {
                    if (header.Key == "Authorization")
                    {
                        var tokenString = CryptoManagerV3.Decrypt(header.Value.ToString().Replace("Bearer", "").Trim(), Global.EncryptionKey);
                        _tokenDto = JsonConvert.DeserializeObject<TokenDto>(tokenString);
                    }
                    else if (header.Key == CookieKeys.AnonymousId && Guid.Parse(header.Value) != Guid.Empty)
                    {
                        _anonymousId = Guid.Parse(header.Value);
                    }
                }
                catch (Exception e)
                {
                    // ignored
                }

            // Authorization header yoksa cookie'den oku
            if (_tokenDto == null && _httpContext.Request.Cookies.TryGetValue("auth_token", out var cookieToken) && !string.IsNullOrEmpty(cookieToken))
            {
                try
                {
                    var tokenString = CryptoManagerV3.Decrypt(cookieToken, Global.EncryptionKey);
                    _tokenDto = JsonConvert.DeserializeObject<TokenDto>(tokenString);
                }
                catch
                {
                    // ignored
                }
            }
        }
        catch (Exception e)
        {
            // ignored
        }
    }
}