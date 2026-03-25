using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Adapters.OneSignalAdapter;

public class OneSignalNotificationService : INotificationService
{
    private readonly HttpClient _httpClient;
    private readonly string _appId;
    private readonly string _restApiKey;
    private readonly ILogger<OneSignalNotificationService> _logger;

    public OneSignalNotificationService(HttpClient httpClient, IConfiguration configuration, ILogger<OneSignalNotificationService> logger)
    {
        _httpClient = httpClient;
        _appId = configuration["OneSignal:AppId"] ?? "";
        _restApiKey = configuration["OneSignal:RestApiKey"] ?? "";
        _logger = logger;
    }

    public async Task SendToUserAsync(Guid userId, string title, string message, Dictionary<string, string>? data = null)
    {
        await SendToUsersAsync(new List<Guid> { userId }, title, message, data);
    }

    public async Task SendToUsersAsync(List<Guid> userIds, string title, string message, Dictionary<string, string>? data = null)
    {
        if (string.IsNullOrEmpty(_appId) || string.IsNullOrEmpty(_restApiKey))
        {
            _logger.LogWarning("OneSignal is not configured. Skipping notification.");
            return;
        }

        try
        {
            var payload = new
            {
                app_id = _appId,
                include_external_user_ids = userIds.Select(id => id.ToString()).ToList(),
                headings = new { en = title },
                contents = new { en = message },
                data = data ?? new Dictionary<string, string>()
            };

            var request = new HttpRequestMessage(HttpMethod.Post, "https://onesignal.com/api/v1/notifications")
            {
                Content = JsonContent.Create(payload)
            };
            request.Headers.Add("Authorization", $"Basic {_restApiKey}");

            var response = await _httpClient.SendAsync(request);

            if (!response.IsSuccessStatusCode)
            {
                var body = await response.Content.ReadAsStringAsync();
                _logger.LogWarning("OneSignal notification failed: {StatusCode} {Body}", response.StatusCode, body);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send OneSignal notification");
        }
    }
}