using Application.Services.Seller.MenuOptionValueService;
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

    public SellerMenuOptionValueController(IMenuOptionValueService menuOptionValueService)
    {
        _menuOptionValueService = menuOptionValueService;
    }

    [HttpGet]
    [AuthorizeAPIRequest(true, false,
        UserRoleEnums.SellerAdmin,
        UserRoleEnums.SellerUser)]
    public async Task<ServiceCollectionResult<MenuOptionValueResponseDto>> GetList([FromQuery] GetMenuOptionValueRequestDto requestDto)
    {
        return await _menuOptionValueService.GetMenuOptionValuesByMenuOptionId(requestDto);
    }

    [HttpPost]
    [AuthorizeAPIRequest(true, false, UserRoleEnums.SellerAdmin)]
    public async Task<ServiceObjectResult<Guid>> Add([FromBody] CreateMenuOptionValueRequestDto requestDto)
    {
        return await _menuOptionValueService.Add(requestDto);
    }

    [HttpPut]
    [AuthorizeAPIRequest(true, false, UserRoleEnums.SellerAdmin)]
    public async Task<ServiceObjectResult<bool>> Update([FromBody] UpdateMenuOptionValueRequestDto requestDto)
    {
        return await _menuOptionValueService.Update(requestDto);
    }

    [HttpDelete]
    [AuthorizeAPIRequest(true, false, UserRoleEnums.SellerAdmin)]
    public async Task<ServiceObjectResult<bool>> Delete([FromBody] DeleteMenuOptionValueRequestDto requestDto)
    {
        return await _menuOptionValueService.Delete(requestDto);
    }
}