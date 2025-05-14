using Microsoft.Extensions.Configuration;

namespace Base.Constant;

public static class Global
{
    public const string EncryptionKey = "AJUNGQMLMWSUWYCK";
    public static readonly string ServiceUrl = Configuration?.GetSection("SiteSettings:ServiceUrl")?.Value ?? string.Empty;
    public static ConfigurationManager? Configuration { get; set; }
}