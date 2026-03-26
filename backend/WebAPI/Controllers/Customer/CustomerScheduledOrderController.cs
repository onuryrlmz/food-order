using Application.Services.Buyer.ScheduledOrderService;
using Base.Enums;
using Domain.Dto.Buyer.ScheduledOrder;
using Domain.Service;
using Microsoft.AspNetCore.Mvc;
using WebAPI.Helpers;

namespace WebAPI.Controllers.Customer;

[Route("v1/customer/scheduled-order")]
[ApiController]
public class CustomerScheduledOrderController : BaseController
{
    private readonly IScheduledOrderService _scheduledOrderService;

    public CustomerScheduledOrderController(IScheduledOrderService scheduledOrderService)
    {
        _scheduledOrderService = scheduledOrderService;
    }

    [HttpPost]
    [AuthorizeAPIRequest(true, false, UserRoleEnums.User)]
    public async Task<ServiceObjectResult<ScheduledOrderDto>> Create([FromBody] CreateScheduledOrderRequestDto requestDto)
    {
        return await _scheduledOrderService.CreateScheduledOrder(requestDto);
    }

    [HttpGet]
    [AuthorizeAPIRequest(true, false, UserRoleEnums.User)]
    public async Task<ServiceCollectionResult<ScheduledOrderDto>> GetMyOrders(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20)
    {
        return await _scheduledOrderService.GetMyScheduledOrders(page, pageSize);
    }

    [HttpGet("{id}")]
    [AuthorizeAPIRequest(true, false, UserRoleEnums.User)]
    public async Task<ServiceObjectResult<ScheduledOrderDetailDto>> GetDetail(Guid id)
    {
        return await _scheduledOrderService.GetScheduledOrderDetail(id);
    }

    [HttpPut("{id}")]
    [AuthorizeAPIRequest(true, false, UserRoleEnums.User)]
    public async Task<ServiceObjectResult<bool>> Update(Guid id, [FromBody] UpdateScheduledOrderRequestDto requestDto)
    {
        return await _scheduledOrderService.UpdateScheduledOrder(id, requestDto);
    }

    [HttpPost("{id}/cancel")]
    [AuthorizeAPIRequest(true, false, UserRoleEnums.User)]
    public async Task<ServiceObjectResult<bool>> Cancel(Guid id, [FromQuery] string? reason)
    {
        return await _scheduledOrderService.CancelScheduledOrder(id, reason);
    }
}
