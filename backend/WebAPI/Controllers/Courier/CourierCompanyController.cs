using Application.Services.Courier.CourierCompanyService;
using Base.Enums;
using Domain.Dto.Courier.Company;
using Domain.Service;
using Microsoft.AspNetCore.Mvc;
using WebAPI.Helpers;

namespace WebAPI.Controllers.Courier;

[Route("v1/courier/company")]
[ApiController]
public class CourierCompanyController : BaseController
{
    private readonly ICourierCompanyService _service;

    public CourierCompanyController(ICourierCompanyService service) => _service = service;

    [HttpPost("register")]
    [AuthorizeAPIRequest(true, false,
        AuthorizationServiceEnums.UserRoleEnums.Courier)]
    public async Task<ServiceObjectResult<CourierCompanyDto>> Register(
        [FromBody] RegisterCourierCompanyRequestDto request)
        => await _service.RegisterCompanyAsync(Client!._tokenDto!.UserId, request);

    [HttpGet("my")]
    [AuthorizeAPIRequest(true, false,
        AuthorizationServiceEnums.UserRoleEnums.CourierCompanyAdmin)]
    public async Task<ServiceObjectResult<CourierCompanyDto>> GetMyCompany()
        => await _service.GetMyCompanyAsync(Client!._tokenDto!.UserId);

    [HttpPut("my")]
    [AuthorizeAPIRequest(true, false,
        AuthorizationServiceEnums.UserRoleEnums.CourierCompanyAdmin)]
    public async Task<ServiceObjectResult<bool>> UpdateCompany(
        [FromBody] UpdateCourierCompanyRequestDto request)
        => await _service.UpdateCompanyAsync(Client!._tokenDto!.UserId, request);

    // Member management
    [HttpGet("members/search")]
    [AuthorizeAPIRequest(true, false,
        AuthorizationServiceEnums.UserRoleEnums.CourierCompanyAdmin)]
    public async Task<ServiceCollectionResult<CourierCompanyMemberDto>> SearchCouriers(
        [FromQuery] string q)
        => await _service.SearchCouriersAsync(q);

    [HttpPost("members/request")]
    [AuthorizeAPIRequest(true, false,
        AuthorizationServiceEnums.UserRoleEnums.CourierCompanyAdmin)]
    public async Task<ServiceObjectResult<bool>> RequestMembership(
        [FromBody] RequestMembershipDto request)
        => await _service.RequestCourierMembershipAsync(Client!._tokenDto!.UserId, request.CourierId);

    [HttpGet("members")]
    [AuthorizeAPIRequest(true, false,
        AuthorizationServiceEnums.UserRoleEnums.CourierCompanyAdmin)]
    public async Task<ServiceCollectionResult<CourierCompanyMemberDto>> GetMembers()
        => await _service.GetCompanyMembersAsync(Client!._tokenDto!.UserId);

    [HttpDelete("members/{id}")]
    [AuthorizeAPIRequest(true, false,
        AuthorizationServiceEnums.UserRoleEnums.CourierCompanyAdmin)]
    public async Task<ServiceObjectResult<bool>> RemoveMember(Guid id)
        => await _service.RemoveMemberAsync(Client!._tokenDto!.UserId, id);

    // Restaurant invites (company side)
    [HttpGet("restaurant-invites")]
    [AuthorizeAPIRequest(true, false,
        AuthorizationServiceEnums.UserRoleEnums.CourierCompanyAdmin)]
    public async Task<ServiceCollectionResult<RestaurantCourierCompanyDto>> GetRestaurantInvites()
        => await _service.GetRestaurantInvitesAsync(Client!._tokenDto!.UserId);

    [HttpPut("restaurant-invites/{id}/accept")]
    [AuthorizeAPIRequest(true, false,
        AuthorizationServiceEnums.UserRoleEnums.CourierCompanyAdmin)]
    public async Task<ServiceObjectResult<bool>> AcceptRestaurantInvite(Guid id)
        => await _service.AcceptRestaurantInviteAsync(Client!._tokenDto!.UserId, id);

    [HttpPut("restaurant-invites/{id}/reject")]
    [AuthorizeAPIRequest(true, false,
        AuthorizationServiceEnums.UserRoleEnums.CourierCompanyAdmin)]
    public async Task<ServiceObjectResult<bool>> RejectRestaurantInvite(Guid id)
        => await _service.RejectRestaurantInviteAsync(Client!._tokenDto!.UserId, id);

    // Pickup flow
    [HttpGet("pickup-orders/{restaurantId}")]
    [AuthorizeAPIRequest(true, false,
        AuthorizationServiceEnums.UserRoleEnums.Courier,
        AuthorizationServiceEnums.UserRoleEnums.CourierCompanyAdmin)]
    public async Task<ServiceCollectionResult<PickupOrderDto>> GetPendingPickupOrders(
        Guid restaurantId, [FromQuery] int page = 1, [FromQuery] int size = 20)
        => await _service.GetPendingPickupOrdersAsync(Client!._tokenDto!.UserId, restaurantId, page, size);

    [HttpPut("pickup/{orderId}/confirm")]
    [AuthorizeAPIRequest(true, false,
        AuthorizationServiceEnums.UserRoleEnums.Courier,
        AuthorizationServiceEnums.UserRoleEnums.CourierCompanyAdmin)]
    public async Task<ServiceObjectResult<bool>> ConfirmPickup(Guid orderId)
        => await _service.ConfirmPickupAsync(Client!._tokenDto!.UserId, orderId);
}
