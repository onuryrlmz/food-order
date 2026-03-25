using Application.Services.Seller.MenuOptionValueOptionService;
using Base.Enums;
using Domain.Dto.Seller.MenuOptionValueOption;
using Domain.Service;
using Microsoft.AspNetCore.Mvc;
using WebAPI.Helpers;

namespace WebAPI.Controllers.Seller;

[Route("v1/seller/menu-option-value-option")]
[ApiController]
public class SellerMenuOptionValueOptionController : BaseController
{
    private readonly IMenuOptionValueOptionService _menuOptionValueOptionService;

    public SellerMenuOptionValueOptionController(IMenuOptionValueOptionService menuOptionValueOptionService)
    {
        _menuOptionValueOptionService = menuOptionValueOptionService;
    }

    [HttpGet]
    [AuthorizeAPIRequest(true, false,
        AuthorizationServiceEnums.UserRoleEnums.SellerAdmin,
        AuthorizationServiceEnums.UserRoleEnums.SellerUser)]
    public async Task<ServiceCollectionResult<MenuOptionValueOptionResponseDto>> GetList([FromQuery] GetMenuOptionValueOptionRequestDto requestDto)
    {
        return await _menuOptionValueOptionService.GetMenuOptionValueOptionsByMenuOptionValueId(requestDto);
    }

    [HttpPost]
    [AuthorizeAPIRequest(true, false, AuthorizationServiceEnums.UserRoleEnums.SellerAdmin)]
    public async Task<ServiceObjectResult<Guid>> Add([FromBody] CreateMenuOptionValueOptionRequestDto requestDto)
    {
        return await _menuOptionValueOptionService.Add(requestDto);
    }

    [HttpPut]
    [AuthorizeAPIRequest(true, false, AuthorizationServiceEnums.UserRoleEnums.SellerAdmin)]
    public async Task<ServiceObjectResult<bool>> Update([FromBody] UpdateMenuOptionValueOptionRequestDto requestDto)
    {
        return await _menuOptionValueOptionService.Update(requestDto);
    }

    [HttpDelete]
    [AuthorizeAPIRequest(true, false, AuthorizationServiceEnums.UserRoleEnums.SellerAdmin)]
    public async Task<ServiceObjectResult<bool>> Delete([FromBody] DeleteMenuOptionValueOptionRequestDto requestDto)
    {
        return await _menuOptionValueOptionService.Delete(requestDto);
    }
}