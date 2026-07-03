using Base.Constant;
using Base.Entities;
using Base.Security;
using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json;

namespace WebAPI.Hubs;

/// <summary>
/// SignalR bağlantısındaki access token'ı (query string 'access_token' veya Authorization header)
/// çözüp doğrular. Böylece hub grup katılımları istemciden gelen kimliğe körü körüne güvenmez.
/// </summary>
internal static class HubAuth
{
    public static TokenDto? GetToken(HubCallerContext context)
    {
        var http = context.GetHttpContext();
        if (http == null) return null;

        string? raw = http.Request.Query["access_token"];
        if (string.IsNullOrEmpty(raw))
        {
            var authHeader = http.Request.Headers["Authorization"].ToString();
            if (!string.IsNullOrEmpty(authHeader))
                raw = authHeader.Replace("Bearer", "").Trim();
        }

        if (string.IsNullOrEmpty(raw)) return null;

        try
        {
            var json = CryptoManagerV3.Decrypt(raw, Global.EncryptionKey);
            if (string.IsNullOrEmpty(json)) return null;

            var token = JsonConvert.DeserializeObject<TokenDto>(json);
            if (token == null || token.Expiration < DateTime.UtcNow) return null;
            return token;
        }
        catch
        {
            return null;
        }
    }
}
