using Application.Services.Buyer.OrderService;
using Application.Services.Courier;
using Base.Enums;
using Domain.Dto.Courier;
using Domain.Service;
using Microsoft.AspNetCore.Mvc;
using WebAPI.Helpers;

namespace WebAPI.Controllers.Courier;

[Route("v1/courier/orders")]
[ApiController]
public class CourierOrderController : BaseController
{
    private readonly ICourierOrderService _courierOrderService;

    public CourierOrderController(ICourierOrderService courierOrderService) => _courierOrderService = courierOrderService;

    [HttpGet("active")]
    [AuthorizeAPIRequest(true, false, AuthorizationServiceEnums.UserRoleEnums.Courier)]
    public async Task<ServiceCollectionResult<CourierOrderDto>> GetActiveOrders()
        => await _courierOrderService.GetActiveOrdersAsync(Client!._tokenDto!.UserId);

    [HttpGet("history")]
    [AuthorizeAPIRequest(true, false, AuthorizationServiceEnums.UserRoleEnums.Courier)]
    public async Task<ServiceCollectionResult<CourierOrderDto>> GetHistory(
        [FromQuery] int month,
        [FromQuery] int year)
        => await _courierOrderService.GetOrderHistoryAsync(Client!._tokenDto!.UserId, month, year);
}
