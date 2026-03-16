using NArchitecture.Core.Persistence.Repositories;

namespace Domain.Entities.Buyer;

public class Payment : Entity<Guid>
{
    public Guid OrderId { get; set; }
    public Guid UserId { get; set; }
    public Guid SellerId { get; set; }
    
    // Tutarlar
    public decimal Amount { get; set; }
    public decimal SellerPayoutAmount { get; set; }
    public decimal CommissionAmount { get; set; }
    
    // Durum
    public short StatusId { get; set; }
    
    // Ödeme sağlayıcı bilgileri
    public string? ProviderPaymentId { get; set; }
    public string? ProviderConversationId { get; set; }
    public string? ProviderTransactionId { get; set; }
    public string? ProviderFraudStatus { get; set; }
    
    // Ödeme yöntemi
    public int PaymentOptionId { get; set; }
    public string? CardLastFourDigits { get; set; }
    public string? CardType { get; set; }
    public string? CardAssociation { get; set; }
    public string? CardAlias { get; set; }
    
    // Hata/durum bilgisi
    public string? ErrorMessage { get; set; }
    public string? ErrorCode { get; set; }
    
    public DateTime? CompletedAt { get; set; }
    public DateTime? FailedAt { get; set; }

    // Refund
    public DateTime? RefundedAt { get; set; }
    public string? RefundTransactionId { get; set; }
    public string? RefundReason { get; set; }
    
    // Navigation
    public virtual Order Order { get; set; }
}
