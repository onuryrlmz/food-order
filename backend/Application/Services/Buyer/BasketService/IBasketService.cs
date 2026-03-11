using Domain.Dto.Buyer;
using Domain.Entities.Buyer;
using Domain.Service;

namespace Application.Services.Buyer.BasketService;

public interface IBasketService
{
    Task<ServiceObjectResult<Basket>> GetBasketByIdForDb();
    Task<ServiceObjectResult<GetBasketDto>> GetBasketByIdForRedis();
    Task<ServiceObjectResult<bool>> UpdateBasketForDb(UpdateBasketDto dto);
    Task<ServiceObjectResult<bool>> UpdateBasketForRedis(UpdateBasketDto dto);
}