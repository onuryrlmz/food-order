namespace Infrastructure.Adapters.AiSupportAdapter;

public class AiSupportResponse
{
    public string Content { get; set; }
    public bool ShouldEscalate { get; set; }
    public AiSuggestedAction? SuggestedAction { get; set; }
    public string? ModelUsed { get; set; }
    public int TokensUsed { get; set; }
}
