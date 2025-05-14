using Base.Entities;
using Domain.Dto.Buyer;
using Domain.Dto.Seller.Menu;
using Domain.Service;

namespace Application.Services.Buyer;

public interface IBasketService
{
    Task<ServiceObjectResult<bool>> GetBasketById();
    Task<ServiceObjectResult<bool>> UpdateBasket(UpdateBasketDto dto);
}