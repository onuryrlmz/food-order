namespace WebAPI.Helpers;

public class WebAPIConfiguration
{
    public string APIDomain { get; set; } = string.Empty;
    public string[] AllowedOrigins { get; set; } = Array.Empty<string>();
}