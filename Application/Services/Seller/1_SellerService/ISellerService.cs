using Domain.Dto.Seller;
using Domain.Service;

namespace Application.Services.Seller._1_SellerService;

public interface ISellerService
{
    Task<ServiceObjectResult<bool>> AddSeller(AddSellerDto requestDto);
}