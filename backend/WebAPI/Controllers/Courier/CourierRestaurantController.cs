using Application.Services.Courier;
using Base.Enums;
using Domain.Dto.Courier;
using Domain.Service;
using Microsoft.AspNetCore.Mvc;
using WebAPI.Helpers;

namespace WebAPI.Controllers.Courier;

[Route("v1/courier/restaurants")]
[ApiController]
public class CourierRestaurantController : BaseController
{
    private readonly ICourierRestaurantService _courierRestaurantService;

    public CourierRestaurantController(ICourierRestaurantService courierRestaurantService)
        => _courierRestaurantService = courierRestaurantService;

    [HttpGet]
    [AuthorizeAPIRequest(true, false,
        AuthorizationServiceEnums.UserRoleEnums.Courier,
        AuthorizationServiceEnums.UserRoleEnums.CourierCompanyAdmin)]
    public async Task<ServiceCollectionResult<CourierRestaurantDto>> GetMyRestaurants()
        => await _courierRestaurantService.GetMyRestaurantsAsync(Client!._tokenDto!.UserId);

    [HttpGet("invites")]
    [AuthorizeAPIRequest(true, false,
        AuthorizationServiceEnums.UserRoleEnums.Courier,
        AuthorizationServiceEnums.UserRoleEnums.CourierCompanyAdmin)]
    public async Task<ServiceCollectionResult<CourierRestaurantDto>> GetPendingInvites()
        => await _courierRestaurantService.GetPendingInvitesAsync(Client!._tokenDto!.UserId);

    [HttpPut("{restaurantCourierId}/accept")]
    [AuthorizeAPIRequest(true, false,
        AuthorizationServiceEnums.UserRoleEnums.Courier,
        AuthorizationServiceEnums.UserRoleEnums.CourierCompanyAdmin)]
    public async Task<ServiceObjectResult<bool>> AcceptInvite(Guid restaurantCourierId)
        => await _courierRestaurantService.AcceptInviteAsync(Client!._tokenDto!.UserId, restaurantCourierId);

    [HttpPut("{restaurantCourierId}/reject")]
    [AuthorizeAPIRequest(true, false,
        AuthorizationServiceEnums.UserRoleEnums.Courier,
        AuthorizationServiceEnums.UserRoleEnums.CourierCompanyAdmin)]
    public async Task<ServiceObjectResult<bool>> RejectInvite(Guid restaurantCourierId)
        => await _courierRestaurantService.RejectInviteAsync(Client!._tokenDto!.UserId, restaurantCourierId);

    [HttpDelete("{restaurantCourierId}")]
    [AuthorizeAPIRequest(true, false,
        AuthorizationServiceEnums.UserRoleEnums.Courier,
        AuthorizationServiceEnums.UserRoleEnums.CourierCompanyAdmin)]
    public async Task<ServiceObjectResult<bool>> LeaveRestaurant(Guid restaurantCourierId)
        => await _courierRestaurantService.LeaveRestaurantAsync(Client!._tokenDto!.UserId, restaurantCourierId);
}
