using Application.Services.Buyer.TipService;
using Base.Enums;
using Domain.Dto.Buyer.Tip;
using Domain.Service;
using Microsoft.AspNetCore.Mvc;
using WebAPI.Helpers;

namespace WebAPI.Controllers.Customer;

[Route("v1/customer/tip")]
[ApiController]
public class CustomerTipController : BaseController
{
    private readonly ITipService _tipService;

    public CustomerTipController(ITipService tipService)
    {
        _tipService = tipService;
    }

    [HttpPost]
    [AuthorizeAPIRequest(true, false, UserRoleEnums.User)]
    public async Task<ServiceObjectResult<TipDto>> AddTip([FromBody] AddTipRequestDto requestDto)
    {
        return await _tipService.AddTip(requestDto);
    }

    [HttpGet("options/{orderId}")]
    [AuthorizeAPIRequest(true, false, UserRoleEnums.User)]
    public async Task<ServiceObjectResult<TipOptionsDto>> GetTipOptions(Guid orderId)
    {
        return await _tipService.GetTipOptions(orderId);
    }

    [HttpGet("order/{orderId}")]
    [AuthorizeAPIRequest(true, false, UserRoleEnums.User)]
    public async Task<ServiceObjectResult<TipDto>> GetTipByOrder(Guid orderId)
    {
        return await _tipService.GetTipByOrder(orderId);
    }
}
