using Application.Services.Buyer.OrderService;
using Base.Enums;
using Domain.Dto.Buyer.Order;
using Domain.Service;
using Microsoft.AspNetCore.Mvc;
using WebAPI.Helpers;

namespace WebAPI.Controllers.Admin;

[Route("v1/admin/order")]
[ApiController]
public class AdminOrderController : BaseController
{
    private readonly IOrderService _orderService;

    public AdminOrderController(IOrderService orderService) => _orderService = orderService;

    [HttpGet("list")]
    [AuthorizeAPIRequest(true, false, AuthorizationServiceEnums.UserRoleEnums.Admin)]
    public async Task<ServiceCollectionResult<GetOrderResponseDto>> GetAll(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] short? statusId = null)
        => await _orderService.GetAllOrdersForAdmin(page, pageSize, statusId);

    [HttpPut("{orderId}/status")]
    [AuthorizeAPIRequest(true, false, AuthorizationServiceEnums.UserRoleEnums.Admin)]
    public async Task<ServiceObjectResult<bool>> UpdateStatus(Guid orderId, [FromQuery] short statusId)
        => await _orderService.UpdateOrderStatus(orderId, statusId);
}
