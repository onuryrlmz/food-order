namespace Infrastructure.Adapters.IyzicoServiceAdapter;

public class ThreeDsCompleteResultDto
{
    public bool Success { get; set; }
    // Iyzico iade işlemleri paymentId'yi değil, ödeme kalemine ait PaymentTransactionId'yi ister.
    public string? PaymentTransactionId { get; set; }
    public string? CardUserKey { get; set; }
    public string? CardToken { get; set; }
    public string? BinNumber { get; set; }
    public string? LastFourDigits { get; set; }
    public string? CardType { get; set; }
    public string? CardAssociation { get; set; }
}
