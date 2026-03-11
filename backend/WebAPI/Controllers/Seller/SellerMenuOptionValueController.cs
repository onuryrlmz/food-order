using Application.Services.Seller._6_MenuOptionValueService;
using Base.Enums;
using Domain.Dto.Seller.MenuOptionValue;
using Domain.Service;
using Microsoft.AspNetCore.Mvc;
using WebAPI.Helpers;

namespace WebAPI.Controllers.Seller;

[Route("v1/seller/menu-option-value")]
[ApiController]
public class SellerMenuOptionValueController : BaseController
{
    private readonly IMenuOptionValueService _menuOptionValueService;

    public SellerMenuOptionValueController(IMenuOptionValueService menuOptionValueService) => _menuOptionValueService = menuOptionValueService;

    [HttpGet]
    [AuthorizeAPIRequest(true, false,
        AuthorizationServiceEnums.UserRoleEnums.SellerAdmin,
        AuthorizationServiceEnums.UserRoleEnums.SellerUser)]
    public async Task<ServiceCollectionResult<MenuOptionValueResponseDto>> GetList([FromQuery] GetMenuOptionValueRequestDto requestDto)
        => await _menuOptionValueService.GetMenuOptionValuesByMenuOptionId(requestDto);

    [HttpPost]
    [AuthorizeAPIRequest(true, false, AuthorizationServiceEnums.UserRoleEnums.SellerAdmin)]
    public async Task<ServiceObjectResult<Guid>> Add([FromBody] CreateMenuOptionValueRequestDto requestDto)
        => await _menuOptionValueService.Add(requestDto);

    [HttpPut]
    [AuthorizeAPIRequest(true, false, AuthorizationServiceEnums.UserRoleEnums.SellerAdmin)]
    public async Task<ServiceObjectResult<bool>> Update([FromBody] UpdateMenuOptionValueRequestDto requestDto)
        => await _menuOptionValueService.Update(requestDto);

    [HttpDelete]
    [AuthorizeAPIRequest(true, false, AuthorizationServiceEnums.UserRoleEnums.SellerAdmin)]
    public async Task<ServiceObjectResult<bool>> Delete([FromBody] DeleteMenuOptionValueRequestDto requestDto)
        => await _menuOptionValueService.Delete(requestDto);
}
