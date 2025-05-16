using Application.Services.Buyer;
using Application.Services.Buyer.BasketService;
using Base.Enums;
using Domain.Dto.Buyer;
using Domain.Entities.Buyer;
using Domain.Service;
using Microsoft.AspNetCore.Mvc;
using WebAPI.Helpers;

namespace WebAPI.Controllers;

[Route("v1/basket")]
[ApiController]
public class BasketController : BaseController
{
    private readonly IBasketService _basketService;

    public BasketController(IBasketService basketService)
    {
        _basketService = basketService;
    }

    [HttpPost("GetBasketById")]
    [AuthorizeAPIRequest(true, false, new[] { AuthorizationServiceEnums.UserRoleEnums.User, AuthorizationServiceEnums.UserRoleEnums.Admin })]
    public async Task<ServiceObjectResult<GetBasketDto>> GetBasketById()
    {
        return await _basketService.GetBasketByIdForRedis();
    }

    [HttpPost("UpdateBasket")]
    [AuthorizeAPIRequest(true, false, new[] { AuthorizationServiceEnums.UserRoleEnums.User, AuthorizationServiceEnums.UserRoleEnums.Admin })]
    public async Task<ServiceObjectResult<bool>> UpdateBasket([FromBody] UpdateBasketDto dto)
    {
        return await _basketService.UpdateBasketForRedis(dto);
    }
}