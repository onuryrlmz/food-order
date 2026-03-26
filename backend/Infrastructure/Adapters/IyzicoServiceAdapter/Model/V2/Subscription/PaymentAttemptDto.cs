namespace Infrastructure.Adapters.IyzicoServiceAdapter.Model.V2.Subscription;

public class PaymentAttemptDto
{
    public string ConversationId { get; set; }
    public string CreatedDate { get; set; }
    public string PaymentId { get; set; }
    public string PaymentStatus { get; set; }
    public string ErrorCode { get; set; }
    public string ErrorMessage { get; set; }
}
