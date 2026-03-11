using Application.Services.Seller._6_MenuService;
using Base.Enums;
using Domain.Dto.Seller.Menu;
using Domain.Service;
using Microsoft.AspNetCore.Mvc;
using WebAPI.Helpers;

namespace WebAPI.Controllers.Seller;

[Route("v1/seller/menu")]
[ApiController]
public class SellerMenuController : BaseController
{
    private readonly IMenuService _menuService;

    public SellerMenuController(IMenuService menuService) => _menuService = menuService;

    [HttpGet("{id}")]
    [AuthorizeAPIRequest(true, false,
        AuthorizationServiceEnums.UserRoleEnums.SellerAdmin,
        AuthorizationServiceEnums.UserRoleEnums.SellerUser)]
    public async Task<ServiceObjectResult<MenuResponseDto>> GetById([FromRoute] Guid id, [FromQuery] Guid restaurantId)
        => await _menuService.GetMenuById(new GetMenuRequestDto { MenuId = id, RestaurantId = restaurantId });

    [HttpGet("by-restaurant/{restaurantId}")]
    [AuthorizeAPIRequest(true, false,
        AuthorizationServiceEnums.UserRoleEnums.SellerAdmin,
        AuthorizationServiceEnums.UserRoleEnums.SellerUser)]
    public async Task<ServiceCollectionResult<MenuResponseDto>> GetByRestaurant(Guid restaurantId)
        => await _menuService.GetMenusByRestaurantId(new GetMenusByRestaurantIdRequestDto { RestaurantId = restaurantId });

    [HttpGet("by-restaurant/{restaurantId}/with-options")]
    [AuthorizeAPIRequest(true, false,
        AuthorizationServiceEnums.UserRoleEnums.SellerAdmin,
        AuthorizationServiceEnums.UserRoleEnums.SellerUser)]
    public async Task<ServiceCollectionResult<MenuResponseDto>> GetByRestaurantWithOptions(Guid restaurantId)
        => await _menuService.GetMenusByRestaurantIdWithOptions(new GetMenusByRestaurantIdRequestDto { RestaurantId = restaurantId });

    [HttpPost]
    [AuthorizeAPIRequest(true, false, AuthorizationServiceEnums.UserRoleEnums.SellerAdmin)]
    public async Task<ServiceObjectResult<Guid>> Create([FromBody] CreateMenuRequestDto requestDto)
        => await _menuService.Create(requestDto);

    [HttpPut]
    [AuthorizeAPIRequest(true, false, AuthorizationServiceEnums.UserRoleEnums.SellerAdmin)]
    public async Task<ServiceObjectResult<bool>> Update([FromBody] UpdateMenuRequestDto requestDto)
        => await _menuService.Update(requestDto);

    [HttpDelete]
    [AuthorizeAPIRequest(true, false, AuthorizationServiceEnums.UserRoleEnums.SellerAdmin)]
    public async Task<ServiceObjectResult<bool>> Delete([FromBody] DeleteMenuRequestDto requestDto)
        => await _menuService.Delete(requestDto);
}
