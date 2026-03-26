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
