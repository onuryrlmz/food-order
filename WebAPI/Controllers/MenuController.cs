using Application.Services.Seller;
using Application.Services.Seller._4_MenuService;
using Base.Enums;
using Domain.Dto.Seller.Menu;
using Domain.Service;
using Microsoft.AspNetCore.Mvc;
using WebAPI.Helpers;

namespace WebAPI.Controllers;

[Route("v1/menu")]
[ApiController]
public class MenuController : BaseController
{
    private readonly IMenuService _menuService;

    public MenuController(IMenuService menuService)
    {
        _menuService = menuService;
    }

    [HttpPost("createMenuInformationJsonFile")]
    [AuthorizeAPIRequest(true, false, new[] { AuthorizationServiceEnums.UserRoleEnums.Admin })]
    public async Task<ServiceObjectResult<bool>> CreateMenuInformationJsonFile([FromBody] GetMenuRequestDto requestDto)
    {
        return await _menuService.CreateMenuInformationJsonFile(requestDto);
    }

    [HttpPost("getMenuInformationByMenuId")]
    [AuthorizeAPIRequest(true, false, new[] { AuthorizationServiceEnums.UserRoleEnums.User })]
    public async Task<ServiceObjectResult<MenuResponseDto>> GetMenuInformationByMenuId([FromBody] GetMenuInformationByMenuIdRequestDto requestDto)
    {
        return await _menuService.GetMenuInformationJsonFileByMenuId(requestDto);
    }

    [HttpPost("getMenusByRestaurantId")]
    public async Task<ServiceCollectionResult<MenuResponseDto>> GetMenusByRestaurantId([FromBody] GetMenusByRestaurantIdRequestDto requestDto)
    {
        return await _menuService.GetMenusByRestaurantId(requestDto);
    }

    [HttpPost("add")]
    public async Task<ServiceObjectResult<Guid>> Add([FromBody] CreateMenuRequestDto requestDto)
    {
        return await _menuService.Create(requestDto);
    }

    [HttpPut("update")]
    public async Task<ServiceObjectResult<bool>> Update([FromBody] UpdateMenuRequestDto requestDto)
    {
        return await _menuService.Update(requestDto);
    }

    [HttpDelete("delete")]
    public async Task<ServiceObjectResult<bool>> Delete([FromBody] DeleteMenuRequestDto requestDto)
    {
        return await _menuService.Delete(requestDto);
    }
}