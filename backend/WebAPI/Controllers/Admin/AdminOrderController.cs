using Application.Services.Buyer.OrderService;
using Application.Services.Buyer.PaymentService;
using Base.Enums;
using Domain.Dto.Admin.Order;
using Domain.Service;
using Microsoft.AspNetCore.Mvc;
using WebAPI.Helpers;

namespace WebAPI.Controllers.Admin;

[Route("v1/admin/order")]
[ApiController]
public class AdminOrderController : BaseController
{
    private readonly IOrderService _orderService;
    private readonly IPaymentService _paymentService;

    public AdminOrderController(IOrderService orderService, IPaymentService paymentService)
    {
        _orderService = orderService;
        _paymentService = paymentService;
    }

    [HttpGet("list")]
    [AuthorizeAPIRequest(true, false, AuthorizationServiceEnums.UserRoleEnums.Admin)]
    public async Task<ServiceCollectionResult<AdminGetOrderResponseDto>> GetAll(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] short? statusId = null)
    {
        return await _orderService.GetAllOrdersForAdmin(page, pageSize, statusId);
    }

    [HttpGet("{orderId}")]
    [AuthorizeAPIRequest(true, false, AuthorizationServiceEnums.UserRoleEnums.Admin)]
    public async Task<ServiceObjectResult<AdminGetOrderResponseDto>> GetDetail(Guid orderId)
    {
        return await _orderService.GetOrderDetailForAdmin(orderId);
    }

    [HttpPut("{orderId}/status")]
    [AuthorizeAPIRequest(true, false, AuthorizationServiceEnums.UserRoleEnums.Admin)]
    public async Task<ServiceObjectResult<bool>> UpdateStatus(Guid orderId, [FromQuery] short statusId)
    {
        return await _orderService.UpdateOrderStatus(orderId, statusId);
    }

    [HttpPost("{orderId}/refund")]
    [AuthorizeAPIRequest(true, false, AuthorizationServiceEnums.UserRoleEnums.Admin)]
    public async Task<ServiceObjectResult<bool>> RefundOrder(Guid orderId, [FromQuery] string? reason = null)
    {
        return await _paymentService.RefundOrderAsync(orderId, reason);
    }

    [HttpGet("overdue")]
    [AuthorizeAPIRequest(true, false, AuthorizationServiceEnums.UserRoleEnums.Admin)]
    public async Task<ServiceCollectionResult<AdminGetOrderResponseDto>> GetOverdueOrders(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20)
    {
        return await _orderService.GetOverdueOrdersForAdmin(page, pageSize);
    }
}