using Application.Services.Buyer.OrderService;
using Application.Services.Courier;
using Application.Services.Seller.CourierService;
using Base.Enums;
using Domain.Dto.Courier;
using Domain.Dto.Seller.Courier;
using Domain.Service;
using Microsoft.AspNetCore.Mvc;
using WebAPI.Helpers;

namespace WebAPI.Controllers.Seller;

[Route("v1/seller")]
[ApiController]
public class SellerCourierController : BaseController
{
    private readonly ICourierService _courierService;
    private readonly IOrderService _orderService;
    private readonly ICourierOrderService _courierOrderService;

    public SellerCourierController(ICourierService courierService, IOrderService orderService, ICourierOrderService courierOrderService)
    {
        _courierService = courierService;
        _orderService = orderService;
        _courierOrderService = courierOrderService;
    }

    [HttpPost("restaurant/{restaurantId}/courier/add")]
    [AuthorizeAPIRequest(true, false,
        AuthorizationServiceEnums.UserRoleEnums.SellerAdmin,
        AuthorizationServiceEnums.UserRoleEnums.SellerUser)]
    public async Task<ServiceObjectResult<bool>> AddCourier(
        Guid restaurantId,
        [FromBody] InviteCourierRequestDto request)
        => await _courierService.AddCourierAsync(restaurantId, request.Email);

    [HttpGet("restaurant/{restaurantId}/couriers")]
    [AuthorizeAPIRequest(true, false,
        AuthorizationServiceEnums.UserRoleEnums.SellerAdmin,
        AuthorizationServiceEnums.UserRoleEnums.SellerUser)]
    public async Task<ServiceCollectionResult<RestaurantCourierDto>> GetCouriers(Guid restaurantId)
        => await _courierService.GetRestaurantCouriersAsync(restaurantId);

    [HttpDelete("restaurant/{restaurantId}/couriers/{courierId}")]
    [AuthorizeAPIRequest(true, false,
        AuthorizationServiceEnums.UserRoleEnums.SellerAdmin,
        AuthorizationServiceEnums.UserRoleEnums.SellerUser)]
    public async Task<ServiceObjectResult<bool>> RemoveCourier(Guid restaurantId, Guid courierId)
        => await _courierService.RemoveCourierAsync(restaurantId, courierId);

    [HttpPut("order/{orderId}/assign-courier")]
    [AuthorizeAPIRequest(true, false,
        AuthorizationServiceEnums.UserRoleEnums.SellerAdmin,
        AuthorizationServiceEnums.UserRoleEnums.SellerUser)]
    public async Task<ServiceObjectResult<bool>> AssignCourier(
        Guid orderId,
        [FromBody] AssignCourierRequestDto request)
        => await _orderService.AssignCourierAsync(orderId, request.CourierId);

    [HttpGet("order/{orderId}/courier-location")]
    [AuthorizeAPIRequest(true, false,
        AuthorizationServiceEnums.UserRoleEnums.SellerAdmin,
        AuthorizationServiceEnums.UserRoleEnums.SellerUser)]
    public async Task<ServiceObjectResult<CourierLocationDto?>> GetCourierLocation(Guid orderId)
        => await _orderService.GetCourierLocationAsync(orderId);

    [HttpGet("courier/{courierId}/orders")]
    [AuthorizeAPIRequest(true, false,
        AuthorizationServiceEnums.UserRoleEnums.SellerAdmin,
        AuthorizationServiceEnums.UserRoleEnums.SellerUser)]
    public async Task<ServiceCollectionResult<CourierOrderDto>> GetCourierOrderHistory(
        Guid courierId,
        [FromQuery] int month,
        [FromQuery] int year)
        => await _courierOrderService.GetOrderHistoryAsync(courierId, month, year);
}
