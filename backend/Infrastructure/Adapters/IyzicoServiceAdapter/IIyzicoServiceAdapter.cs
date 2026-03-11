using Domain.Dto.Buyer.Order;
using Domain.Dto.Payment;
using Domain.Service;

namespace Infrastructure.Adapters.IyzicoServiceAdapter;

public interface IIyzicoServiceAdapter
{
    ServiceObjectResult<string> CreateSeller(CreateSubMerchantDto requestDto);
    ServiceObjectResult<bool> UpdateSeller(UpdateSellerRequestDto requestDto);
    Task<ServiceObjectResult<InitiatePaymentResponseDto>> InitiatePayment(IyzicoPaymentRequestDto requestDto);
    Task<ServiceObjectResult<bool>> CompleteThreeDsPayment(string conversationId, string paymentId, string conversationData);
}
