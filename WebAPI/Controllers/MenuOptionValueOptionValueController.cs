using Application.Services.Seller;
using Application.Services.Seller._8_MenuOptionValueOptionValueService;
using Domain.Dto.Seller.MenuOptionValueOptionValue;
using Domain.Service;
using Microsoft.AspNetCore.Mvc;
using WebAPI.Helpers;

namespace WebAPI.Controllers;

[Route("v1/menuOptionValueOptionValue")]
[ApiController]
public class MenuOptionValueOptionValueController : BaseController
{
    private readonly IMenuOptionValueOptionValueService _menuOptionValueOptionValueService;

    public MenuOptionValueOptionValueController(IMenuOptionValueOptionValueService menuOptionValueOptionValueService)
    {
        _menuOptionValueOptionValueService = menuOptionValueOptionValueService;
    }

    [HttpPost("GetMenuOptionValueOptionValuesByMenuOptionValueOptionId")]
    public async Task<ServiceCollectionResult<MenuOptionValueOptionValueResponseDto>> GetMenuOptionValueOptionValuesByMenuOptionValueOptionId([FromBody] GetMenuOptionValueOptionValueRequestDto requestDto)
    {
        return await _menuOptionValueOptionValueService.GetMenuOptionValueOptionValueByMenuOptionValueOptionId(requestDto);
    }

    [HttpPost("add")]
    public async Task<ServiceObjectResult<Guid>> Add([FromBody] CreateMenuOptionValueOptionValueRequestDto requestDto)
    {
        return await _menuOptionValueOptionValueService.Create(requestDto);
    }

    [HttpPut("update")]
    public async Task<ServiceObjectResult<bool>> Update([FromBody] UpdateMenuOptionValueOptionValueRequestDto requestDto)
    {
        return await _menuOptionValueOptionValueService.Update(requestDto);
    }

    [HttpDelete("delete")]
    public async Task<ServiceObjectResult<bool>> Delete([FromBody] DeleteMenuOptionValueOptionValueRequestDto requestDto)
    {
        return await _menuOptionValueOptionValueService.Delete(requestDto);
    }
}