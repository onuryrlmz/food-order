using Application.Services.Seller._7_MenuOptionService;
using Base.Enums;
using Domain.Dto.Seller.MenuOption;
using Domain.Service;
using Microsoft.AspNetCore.Mvc;
using WebAPI.Helpers;

namespace WebAPI.Controllers.Seller;

[Route("v1/seller/menu-option")]
[ApiController]
public class SellerMenuOptionController : BaseController
{
    private readonly IMenuOptionService _menuOptionService;

    public SellerMenuOptionController(IMenuOptionService menuOptionService) => _menuOptionService = menuOptionService;

    [HttpGet]
    [AuthorizeAPIRequest(true, false,
        AuthorizationServiceEnums.UserRoleEnums.SellerAdmin,
        AuthorizationServiceEnums.UserRoleEnums.SellerUser)]
    public async Task<ServiceCollectionResult<MenuOptionResponseDto>> GetList([FromQuery] GetMenuOptionsRequestDto requestDto)
        => await _menuOptionService.GetMenuOptionsByMenuId(requestDto);

    [HttpPost]
    [AuthorizeAPIRequest(true, false, AuthorizationServiceEnums.UserRoleEnums.SellerAdmin)]
    public async Task<ServiceObjectResult<Guid>> Add([FromBody] CreateMenuOptionRequestDto requestDto)
        => await _menuOptionService.Add(requestDto);

    [HttpPut]
    [AuthorizeAPIRequest(true, false, AuthorizationServiceEnums.UserRoleEnums.SellerAdmin)]
    public async Task<ServiceObjectResult<bool>> Update([FromBody] UpdateMenuOptionRequestDto requestDto)
        => await _menuOptionService.Update(requestDto);

    [HttpDelete]
    [AuthorizeAPIRequest(true, false, AuthorizationServiceEnums.UserRoleEnums.SellerAdmin)]
    public async Task<ServiceObjectResult<bool>> Delete([FromBody] DeleteMenuOptionRequestDto requestDto)
        => await _menuOptionService.Delete(requestDto);
}
