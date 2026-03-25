using Application.Services.Buyer.OrderService;
using Application.Services.Courier.DeliveryAssignmentService;
using Base.Enums;
using Domain.Dto.Buyer.Order;
using Domain.Dto.Courier;
using Domain.Service;
using Microsoft.AspNetCore.Mvc;
using WebAPI.Helpers;

namespace WebAPI.Controllers.Customer;

[Route("v1/customer/order")]
[ApiController]
public class CustomerOrderController : BaseController
{
    private readonly IOrderService _orderService;
    private readonly IDeliveryAssignmentService _deliveryAssignmentService;

    public CustomerOrderController(IOrderService orderService, IDeliveryAssignmentService deliveryAssignmentService)
    {
        _orderService = orderService;
        _deliveryAssignmentService = deliveryAssignmentService;
    }

    [HttpPost("place")]
    [AuthorizeAPIRequest(true, false, AuthorizationServiceEnums.UserRoleEnums.User)]
    public async Task<ServiceObjectResult<PlaceOrderResponseDto>> PlaceOrder([FromBody] PlaceOrderRequestDto requestDto)
        => await _orderService.PlaceOrder(requestDto);

    [HttpGet("active")]
    [AuthorizeAPIRequest(true, false, AuthorizationServiceEnums.UserRoleEnums.User)]
    public async Task<ServiceCollectionResult<GetOrderResponseDto>> GetActive()
        => await _orderService.GetActiveOrders();

    [HttpGet("history")]
    [AuthorizeAPIRequest(true, false, AuthorizationServiceEnums.UserRoleEnums.User)]
    public async Task<ServiceCollectionResult<GetOrderResponseDto>> GetHistory(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20)
        => await _orderService.GetOrderHistory(page, pageSize);

    [HttpGet("{orderId}")]
    [AuthorizeAPIRequest(true, false, AuthorizationServiceEnums.UserRoleEnums.User)]
    public async Task<ServiceObjectResult<GetOrderResponseDto>> GetById(Guid orderId)
        => await _orderService.GetOrderById(orderId);

    [HttpPost("{orderId}/cancel")]
    [AuthorizeAPIRequest(true, false, AuthorizationServiceEnums.UserRoleEnums.User)]
    public async Task<ServiceObjectResult<bool>> Cancel(Guid orderId, [FromQuery] string? reason = null)
        => await _orderService.CancelOrder(orderId, reason ?? string.Empty);

    [HttpPost("{orderId}/payment/initiate")]
    [AuthorizeAPIRequest(true, false, AuthorizationServiceEnums.UserRoleEnums.User)]
    public async Task<ServiceObjectResult<InitiatePaymentResponseDto>> InitiatePayment(
        Guid orderId, [FromBody] InitiatePaymentRequestDto requestDto)
        => await _orderService.InitiatePayment(orderId, requestDto);

    [HttpPost("{orderId}/reorder")]
    [AuthorizeAPIRequest(true, false, AuthorizationServiceEnums.UserRoleEnums.User)]
    public async Task<ServiceObjectResult<ReorderResponseDto>> Reorder(Guid orderId)
        => await _orderService.ReorderAsync(orderId);

    [HttpGet("{orderId}/tracking")]
    [AuthorizeAPIRequest(true, false, AuthorizationServiceEnums.UserRoleEnums.User)]
    public async Task<ServiceObjectResult<CourierTrackingResponseDto>> GetTracking(Guid orderId)
        => await _deliveryAssignmentService.GetOrderTracking(orderId);
}
