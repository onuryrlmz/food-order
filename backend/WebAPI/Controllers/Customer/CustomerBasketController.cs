using Application.Services.Buyer.BasketService;
using Base.Enums;
using Domain.Dto.Buyer;
using Domain.Entities.Buyer;
using Domain.Service;
using Microsoft.AspNetCore.Mvc;
using WebAPI.Helpers;

namespace WebAPI.Controllers.Customer;

[Route("v1/customer/basket")]
[ApiController]
public class CustomerBasketController : BaseController
{
    private readonly IBasketService _basketService;

    public CustomerBasketController(IBasketService basketService)
    {
        _basketService = basketService;
    }

    [HttpGet]
    [AuthorizeAPIRequest(true, false, AuthorizationServiceEnums.UserRoleEnums.User)]
    public async Task<ServiceObjectResult<GetBasketDto>> GetBasket()
    {
        return await _basketService.GetBasketByIdForRedis();
    }

    [HttpPut]
    [AuthorizeAPIRequest(true, false, AuthorizationServiceEnums.UserRoleEnums.User)]
    public async Task<ServiceObjectResult<bool>> UpdateBasket([FromBody] UpdateBasketDto requestDto)
    {
        return await _basketService.UpdateBasketForRedis(requestDto);
    }

    [HttpDelete]
    [AuthorizeAPIRequest(true, false, AuthorizationServiceEnums.UserRoleEnums.User)]
    public async Task<ServiceObjectResult<bool>> ClearBasket()
    {
        return await _basketService.ClearBasketForRedis();
    }
}