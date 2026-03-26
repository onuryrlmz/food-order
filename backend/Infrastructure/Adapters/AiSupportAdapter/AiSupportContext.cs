namespace Infrastructure.Adapters.AiSupportAdapter;

public class AiSupportContext
{
    public List<AiConversationMessage> ConversationHistory { get; set; } = new();
    public string? OrderSummaryJson { get; set; }
    public string? RestaurantInfoJson { get; set; }
    public string Topic { get; set; }
}
