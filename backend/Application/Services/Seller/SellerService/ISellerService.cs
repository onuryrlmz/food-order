using Domain.Dto.Common;
using Domain.Dto.Seller;
using Domain.Service;

namespace Application.Services.Seller.SellerService;

public interface ISellerService
{
    Task<ServiceObjectResult<bool>> AddSeller(AddSellerDto requestDto);
    Task<ServiceObjectResult<bool>> ConfirmSeller(ConfirmSellerDto requestDto);
    Task<ServiceCollectionResult<GetSellerListResponseDto>> GetSellerList(int page = 1, int pageSize = 20);
    Task<ServiceObjectResult<bool>> UpdateSeller(UpdateSellerDto requestDto);
    Task<ServiceObjectResult<bool>> RetryIyzicoRegistrationAsync(Guid sellerId);
}