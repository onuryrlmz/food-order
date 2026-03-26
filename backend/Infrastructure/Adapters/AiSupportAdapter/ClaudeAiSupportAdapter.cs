using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Configuration;

namespace Infrastructure.Adapters.AiSupportAdapter;

public class ClaudeAiSupportAdapter : IAiSupportAdapter
{
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _configuration;

    public ClaudeAiSupportAdapter(HttpClient httpClient, IConfiguration configuration)
    {
        _httpClient = httpClient;
        _configuration = configuration;
    }

    public async Task<AiSupportResponse> GetResponseAsync(AiSupportContext context)
    {
        var apiKey = _configuration["AiSupport:ApiKey"];
        var model = _configuration["AiSupport:Model"] ?? "claude-sonnet-4-20250514";
        var maxTokens = int.Parse(_configuration["AiSupport:MaxTokens"] ?? "1024");

        var systemPrompt = BuildSystemPrompt(context);
        var messages = context.ConversationHistory.Select(m => new
        {
            role = m.Role,
            content = m.Content
        }).ToList();

        var requestBody = new
        {
            model,
            max_tokens = maxTokens,
            system = systemPrompt,
            messages
        };

        var request = new HttpRequestMessage(HttpMethod.Post, "https://api.anthropic.com/v1/messages");
        request.Headers.Add("x-api-key", apiKey);
        request.Headers.Add("anthropic-version", "2023-06-01");
        request.Content = new StringContent(JsonSerializer.Serialize(requestBody), Encoding.UTF8, "application/json");

        var response = await _httpClient.SendAsync(request);
        var responseBody = await response.Content.ReadAsStringAsync();

        if (!response.IsSuccessStatusCode)
        {
            return new AiSupportResponse
            {
                Content = "Uzgunuz, su anda destek hizmetimizde bir sorun yasanmaktadir. Lutfen daha sonra tekrar deneyiniz.",
                ShouldEscalate = true,
                ModelUsed = model,
                TokensUsed = 0
            };
        }

        var jsonDoc = JsonDocument.Parse(responseBody);
        var root = jsonDoc.RootElement;

        var contentText = "";
        if (root.TryGetProperty("content", out var contentArray))
        {
            foreach (var block in contentArray.EnumerateArray())
            {
                if (block.GetProperty("type").GetString() == "text")
                {
                    contentText = block.GetProperty("text").GetString() ?? "";
                    break;
                }
            }
        }

        var tokensUsed = 0;
        if (root.TryGetProperty("usage", out var usage))
        {
            tokensUsed = usage.TryGetProperty("output_tokens", out var outputTokens)
                ? outputTokens.GetInt32()
                : 0;
        }

        // Parse AI response for escalation signals and action suggestions
        var shouldEscalate = contentText.Contains("[ESCALATE]", StringComparison.OrdinalIgnoreCase);
        AiSuggestedAction? suggestedAction = null;

        if (contentText.Contains("[CANCEL_ORDER:", StringComparison.OrdinalIgnoreCase))
        {
            suggestedAction = new AiSuggestedAction
            {
                ActionType = 1, // OrderCancelled
                ActionDataJson = ExtractActionData(contentText, "[CANCEL_ORDER:")
            };
        }

        // Clean action tags from visible content
        contentText = CleanActionTags(contentText);

        return new AiSupportResponse
        {
            Content = contentText,
            ShouldEscalate = shouldEscalate,
            SuggestedAction = suggestedAction,
            ModelUsed = model,
            TokensUsed = tokensUsed
        };
    }

    private string BuildSystemPrompt(AiSupportContext context)
    {
        var sb = new StringBuilder();
        sb.AppendLine("Sen bir yemek siparis platformunun AI musteri destek asistanisin.");
        sb.AppendLine("Turkce yanit ver. Nazik, profesyonel ve cozum odakli ol.");
        sb.AppendLine();
        sb.AppendLine("Gorevlerin:");
        sb.AppendLine("- Musteri sorularini yanitla");
        sb.AppendLine("- Siparis sorunlarini coz");
        sb.AppendLine("- Sistemde iade mekanizmasi yoktur. Musteri siparisi sadece 'Hazirlaniyor' durumuna gecmeden iptal edebilir.");
        sb.AppendLine("- Restoran siparisi reddedebilir (RejectedByRestaurant).");
        sb.AppendLine("- Siparis iptali icin [CANCEL_ORDER:{\"orderId\":\"...\"}] formatinda aksiyon oner (sadece iptal edilebilir durumdaki siparisler icin)");
        sb.AppendLine("- Cozemedigin durumlarda [ESCALATE] ile yetkili birime yonlendir");
        sb.AppendLine("- Iade talebi geldiginde, sistemde iade olmadagini nazikce acikla ve gerekirse yetkili birime yonlendir.");
        sb.AppendLine();
        sb.AppendLine($"Konu: {context.Topic}");

        if (!string.IsNullOrEmpty(context.OrderSummaryJson))
        {
            sb.AppendLine();
            sb.AppendLine($"Ilgili siparis bilgisi: {context.OrderSummaryJson}");
        }

        if (!string.IsNullOrEmpty(context.RestaurantInfoJson))
        {
            sb.AppendLine();
            sb.AppendLine($"Restoran bilgisi: {context.RestaurantInfoJson}");
        }

        return sb.ToString();
    }

    private static string ExtractActionData(string content, string tag)
    {
        var startIdx = content.IndexOf(tag, StringComparison.OrdinalIgnoreCase);
        if (startIdx < 0) return "{}";

        startIdx += tag.Length;
        var endIdx = content.IndexOf(']', startIdx);
        if (endIdx < 0) return "{}";

        return content.Substring(startIdx, endIdx - startIdx);
    }

    private static string CleanActionTags(string content)
    {
        // Remove action tags like [CANCEL_ORDER:...], [ESCALATE]
        var result = content;
        while (true)
        {
            var start = result.IndexOf('[');
            if (start < 0) break;

            var end = result.IndexOf(']', start);
            if (end < 0) break;

            var tag = result.Substring(start, end - start + 1);
            if (tag.StartsWith("[CANCEL_ORDER:", StringComparison.OrdinalIgnoreCase) ||
                tag.Equals("[ESCALATE]", StringComparison.OrdinalIgnoreCase))
            {
                result = result.Remove(start, end - start + 1);
            }
            else
            {
                break; // Avoid infinite loop on non-action brackets
            }
        }

        return result.Trim();
    }
}
