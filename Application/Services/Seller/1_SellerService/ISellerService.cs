using Domain.Dto.Common;
using Domain.Dto.Seller;
using Domain.Service;

namespace Application.Services.Seller._1_SellerService;

public interface ISellerService
{
    Task<ServiceObjectResult<bool>> AddSeller(AddSellerDto requestDto);
    Task<ServiceObjectResult<bool>> ConfirmSeller(ConfirmSellerDto requestDto);
}