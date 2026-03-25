using Application.Services.Buyer.OrderService;
using Base.Enums;
using Domain.Dto.Buyer.Order;
using Domain.Service;
using Microsoft.AspNetCore.Mvc;
using WebAPI.Helpers;

namespace WebAPI.Controllers.Seller;

[Route("v1/seller/order")]
[ApiController]
public class SellerOrderController : BaseController
{
    private readonly IOrderService _orderService;

    public SellerOrderController(IOrderService orderService)
    {
        _orderService = orderService;
    }

    [HttpGet("restaurant/{restaurantId}")]
    [AuthorizeAPIRequest(true, false,
        AuthorizationServiceEnums.UserRoleEnums.SellerAdmin,
        AuthorizationServiceEnums.UserRoleEnums.SellerUser,
        AuthorizationServiceEnums.UserRoleEnums.Admin)]
    public async Task<ServiceCollectionResult<GetOrderResponseDto>> GetByRestaurant(
        Guid restaurantId,
        [FromQuery] short? statusId = null,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20)
    {
        return await _orderService.GetRestaurantOrders(restaurantId, statusId, page, pageSize);
    }

    [HttpGet("{orderId}")]
    [AuthorizeAPIRequest(true, false,
        AuthorizationServiceEnums.UserRoleEnums.SellerAdmin,
        AuthorizationServiceEnums.UserRoleEnums.SellerUser,
        AuthorizationServiceEnums.UserRoleEnums.Admin)]
    public async Task<ServiceObjectResult<GetOrderResponseDto>> GetById(Guid orderId)
    {
        return await _orderService.GetOrderById(orderId);
    }

    [HttpPut("{orderId}/status")]
    [AuthorizeAPIRequest(true, false,
        AuthorizationServiceEnums.UserRoleEnums.SellerAdmin,
        AuthorizationServiceEnums.UserRoleEnums.Admin)]
    public async Task<ServiceObjectResult<bool>> UpdateStatus(Guid orderId, [FromQuery] short statusId)
    {
        return await _orderService.UpdateOrderStatus(orderId, statusId);
    }
}