namespace Infrastructure.Adapters.IyzicoServiceAdapter;

public class RefundResultDto
{
    public bool Success { get; set; }
    public string? TransactionId { get; set; }
    public string? ErrorMessage { get; set; }
}
