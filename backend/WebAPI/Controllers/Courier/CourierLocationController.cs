using Application.Services.Courier;
using Base.Enums;
using Domain.Dto.Courier;
using Domain.Service;
using Microsoft.AspNetCore.Mvc;
using WebAPI.Helpers;

namespace WebAPI.Controllers.Courier;

[Route("v1/courier")]
[ApiController]
public class CourierLocationController : BaseController
{
    private readonly ICourierLocationService _courierLocationService;

    public CourierLocationController(ICourierLocationService courierLocationService)
        => _courierLocationService = courierLocationService;

    [HttpPost("location")]
    [AuthorizeAPIRequest(true, false, AuthorizationServiceEnums.UserRoleEnums.Courier)]
    public async Task<ServiceObjectResult<bool>> UpdateLocation([FromBody] UpdateLocationRequestDto request)
        => await _courierLocationService.UpdateLocationAsync(Client!._tokenDto!.UserId, request.Latitude, request.Longitude, request.OrderId);
}
