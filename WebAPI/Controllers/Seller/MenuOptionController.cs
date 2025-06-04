using Application.Services.Seller._7_MenuOptionService;
using Domain.Dto.Seller.MenuOption;
using Domain.Service;
using Microsoft.AspNetCore.Mvc;
using WebAPI.Helpers;

namespace WebAPI.Controllers.Seller;

[Route("v1/seller/menuOption")]
[ApiController]
public class MenuOptionController : BaseController
{
    private readonly IMenuOptionService _menuOptionService;

    public MenuOptionController(IMenuOptionService menuOptionService)
    {
        _menuOptionService = menuOptionService;
    }

    [HttpPost("getMenuOptionsByMenuId")]
    public async Task<ServiceCollectionResult<MenuOptionResponseDto>> GetMenuOptionsByMenuId([FromBody] GetMenuOptionsRequestDto requestDto)
    {
        return await _menuOptionService.GetMenuOptionsByMenuId(requestDto);
    }

    [HttpPost("add")]
    public async Task<ServiceObjectResult<Guid>> Add([FromBody] CreateMenuOptionRequestDto requestDto)
    {
        return await _menuOptionService.Add(requestDto);
    }

    [HttpPut("update")]
    public async Task<ServiceObjectResult<bool>> Update([FromBody] UpdateMenuOptionRequestDto requestDto)
    {
        return await _menuOptionService.Update(requestDto);
    }

    [HttpDelete("delete")]
    public async Task<ServiceObjectResult<bool>> Delete([FromBody] DeleteMenuOptionRequestDto requestDto)
    {
        return await _menuOptionService.Delete(requestDto);
    }
}