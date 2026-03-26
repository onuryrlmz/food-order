namespace Domain.Dto.Admin.Order;

public class AdminPaymentDto
{
    public Guid Id { get; set; }
    public decimal Amount { get; set; }
    public decimal SellerPayoutAmount { get; set; }
    public decimal CommissionAmount { get; set; }
    public short StatusId { get; set; }
    public string StatusName { get; set; }
    public string? ProviderPaymentId { get; set; }
    public string? ProviderConversationId { get; set; }
    public string? ProviderTransactionId { get; set; }
    public int PaymentOptionId { get; set; }
    public string? CardLastFourDigits { get; set; }
    public string? CardType { get; set; }
    public string? CardAssociation { get; set; }
    public string? ErrorMessage { get; set; }
    public string? ErrorCode { get; set; }
    public DateTime? CompletedAt { get; set; }
    public DateTime? FailedAt { get; set; }
    public DateTime CreatedDate { get; set; }
}
