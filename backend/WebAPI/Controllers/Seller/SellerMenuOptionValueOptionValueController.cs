using Application.Services.Seller._8_MenuOptionValueOptionValueService;
using Base.Enums;
using Domain.Dto.Seller.MenuOptionValueOptionValue;
using Domain.Service;
using Microsoft.AspNetCore.Mvc;
using WebAPI.Helpers;

namespace WebAPI.Controllers.Seller;

[Route("v1/seller/menu-option-value-option-value")]
[ApiController]
public class SellerMenuOptionValueOptionValueController : BaseController
{
    private readonly IMenuOptionValueOptionValueService _menuOptionValueOptionValueService;

    public SellerMenuOptionValueOptionValueController(IMenuOptionValueOptionValueService menuOptionValueOptionValueService) => _menuOptionValueOptionValueService = menuOptionValueOptionValueService;

    [HttpGet]
    [AuthorizeAPIRequest(true, false,
        AuthorizationServiceEnums.UserRoleEnums.SellerAdmin,
        AuthorizationServiceEnums.UserRoleEnums.SellerUser)]
    public async Task<ServiceCollectionResult<MenuOptionValueOptionValueResponseDto>> GetList([FromQuery] GetMenuOptionValueOptionValueRequestDto requestDto)
        => await _menuOptionValueOptionValueService.GetMenuOptionValueOptionValueByMenuOptionValueOptionId(requestDto);

    [HttpPost]
    [AuthorizeAPIRequest(true, false, AuthorizationServiceEnums.UserRoleEnums.SellerAdmin)]
    public async Task<ServiceObjectResult<Guid>> Create([FromBody] CreateMenuOptionValueOptionValueRequestDto requestDto)
        => await _menuOptionValueOptionValueService.Create(requestDto);

    [HttpPut]
    [AuthorizeAPIRequest(true, false, AuthorizationServiceEnums.UserRoleEnums.SellerAdmin)]
    public async Task<ServiceObjectResult<bool>> Update([FromBody] UpdateMenuOptionValueOptionValueRequestDto requestDto)
        => await _menuOptionValueOptionValueService.Update(requestDto);

    [HttpDelete]
    [AuthorizeAPIRequest(true, false, AuthorizationServiceEnums.UserRoleEnums.SellerAdmin)]
    public async Task<ServiceObjectResult<bool>> Delete([FromBody] DeleteMenuOptionValueOptionValueRequestDto requestDto)
        => await _menuOptionValueOptionValueService.Delete(requestDto);
}
