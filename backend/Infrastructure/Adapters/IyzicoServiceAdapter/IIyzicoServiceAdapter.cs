using Domain.Dto.Buyer.Order;
using Domain.Dto.Payment;
using Domain.Service;

namespace Infrastructure.Adapters.IyzicoServiceAdapter;

public interface IIyzicoServiceAdapter
{
    ServiceObjectResult<string> CreateSeller(CreateSubMerchantDto requestDto);
    ServiceObjectResult<bool> UpdateSeller(UpdateSellerRequestDto requestDto);
    Task<ServiceObjectResult<InitiatePaymentResponseDto>> InitiatePayment(IyzicoPaymentRequestDto requestDto);
    Task<ServiceObjectResult<ThreeDsCompleteResultDto>> CompleteThreeDsPayment(string conversationId, string paymentId, string conversationData);

    // Refund
    ServiceObjectResult<RefundResultDto> RefundPayment(string paymentTransactionId, decimal amount);

    // Card storage
    ServiceObjectResult<CardStorageResultDto> CreateCard(string externalId, string email, string? cardUserKey, string cardAlias, string cardNumber, string expireYear, string expireMonth, string cardHolderName);
    ServiceObjectResult<List<CardDetailDto>> GetCards(string cardUserKey);
    ServiceObjectResult<bool> DeleteCard(string cardUserKey, string cardToken);
}

public class ThreeDsCompleteResultDto
{
    public bool Success { get; set; }
    public string? CardUserKey { get; set; }
    public string? CardToken { get; set; }
    public string? BinNumber { get; set; }
    public string? LastFourDigits { get; set; }
    public string? CardType { get; set; }
    public string? CardAssociation { get; set; }
}

public class RefundResultDto
{
    public bool Success { get; set; }
    public string? TransactionId { get; set; }
    public string? ErrorMessage { get; set; }
}

public class CardStorageResultDto
{
    public string CardUserKey { get; set; }
    public string CardToken { get; set; }
    public string BinNumber { get; set; }
    public string LastFourDigits { get; set; }
    public string CardType { get; set; }
    public string CardAssociation { get; set; }
    public string CardFamily { get; set; }
    public string CardAlias { get; set; }
    public string CardBankName { get; set; }
}

public class CardDetailDto
{
    public string CardToken { get; set; }
    public string CardAlias { get; set; }
    public string BinNumber { get; set; }
    public string LastFourDigits { get; set; }
    public string CardType { get; set; }
    public string CardAssociation { get; set; }
    public string CardFamily { get; set; }
    public string CardBankName { get; set; }
    public long? CardBankCode { get; set; }
    public string ExpireMonth { get; set; }
    public string ExpireYear { get; set; }
}