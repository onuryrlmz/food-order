using Application.Services.Seller;
using Domain.Dto.Seller.MenuOptionValue;
using Domain.Service;
using Microsoft.AspNetCore.Mvc;
using WebAPI.Helpers;

namespace WebAPI.Controllers;

[Route("v1/menuOptionValue")]
[ApiController]
public class MenuOptionValueController : BaseController
{
    private readonly IMenuOptionValueService _menuOptionValueService;

    public MenuOptionValueController(IMenuOptionValueService menuOptionValueService)
    {
        _menuOptionValueService = menuOptionValueService;
    }

    [HttpPost("getMenuOptionValuesByMenuOptionId")]
    public async Task<ServiceCollectionResult<MenuOptionValueResponseDto>> GetMenuOptionValuesByMenuOptionId([FromBody] GetMenuOptionValueRequestDto requestDto)
    {
        return await _menuOptionValueService.GetMenuOptionValuesByMenuOptionId(requestDto);
    }

    [HttpPost("add")]
    public async Task<ServiceObjectResult<Guid>> Add([FromBody] CreateMenuOptionValueRequestDto requestDto)
    {
        return await _menuOptionValueService.Add(requestDto);
    }

    [HttpPut("update")]
    public async Task<ServiceObjectResult<bool>> Update([FromBody] UpdateMenuOptionValueRequestDto requestDto)
    {
        return await _menuOptionValueService.Update(requestDto);
    }

    [HttpDelete("delete")]
    public async Task<ServiceObjectResult<bool>> Delete([FromBody] DeleteMenuOptionValueRequestDto requestDto)
    {
        return await _menuOptionValueService.Delete(requestDto);
    }
}