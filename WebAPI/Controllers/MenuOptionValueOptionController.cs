using Application.Services.Seller._7_MenuOptionValueOptionService;
using Domain.Dto.Seller.MenuOptionValueOption;
using Domain.Service;
using Microsoft.AspNetCore.Mvc;
using WebAPI.Helpers;

namespace WebAPI.Controllers;

[Route("api/menuOptionValueOption")]
[ApiController]
public class MenuOptionValueOptionController : BaseController
{
    private readonly IMenuOptionValueOptionService _menuOptionValueOptionService;

    public MenuOptionValueOptionController(IMenuOptionValueOptionService menuOptionValueOptionService)
    {
        _menuOptionValueOptionService = menuOptionValueOptionService;
    }

    [HttpPost("GetMenuOptionValueOptionsByMenuOptionValueId")]
    public async Task<ServiceCollectionResult<MenuOptionValueOptionResponseDto>> GetMenuOptionValueOptionsByMenuOptionValueId([FromBody] GetMenuOptionValueOptionRequestDto requestDto)
    {
        return await _menuOptionValueOptionService.GetMenuOptionValueOptionsByMenuOptionValueId(requestDto);
    }

    [HttpPost("add")]
    public async Task<ServiceObjectResult<Guid>> Add([FromBody] CreateMenuOptionValueOptionRequestDto requestDto)
    {
        return await _menuOptionValueOptionService.Add(requestDto);
    }

    [HttpPut("update")]
    public async Task<ServiceObjectResult<bool>> Update([FromBody] UpdateMenuOptionValueOptionRequestDto requestDto)
    {
        return await _menuOptionValueOptionService.Update(requestDto);
    }

    [HttpDelete("delete")]
    public async Task<ServiceObjectResult<bool>> Delete([FromBody] DeleteMenuOptionValueOptionRequestDto requestDto)
    {
        return await _menuOptionValueOptionService.Delete(requestDto);
    }
}