using Domain.Dto.Payment;
using Domain.Service;

namespace Infrastructure.Adapters.IyzicoServiceAdapter;

public interface IIyzicoServiceAdapter
{
    ServiceObjectResult<string> CreateSeller(CreateSubMerchantDto requestDto);
    ServiceObjectResult<bool> UpdateSeller(UpdateSellerRequestDto requestDto);
}