namespace Infrastructure.Adapters.AiSupportAdapter;

public interface IAiSupportAdapter
{
    Task<AiSupportResponse> GetResponseAsync(AiSupportContext context);
}
