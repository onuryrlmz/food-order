using Domain.Dto.Seller;
using Domain.Service;

namespace Application.Services.Seller;

public interface ISellerService
{
    Task<ServiceObjectResult<bool>> AddSeller(AddSellerDto requestDto);
}