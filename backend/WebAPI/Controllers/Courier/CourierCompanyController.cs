using Application.Services.Courier.CourierCompanyService;
using Base.Enums;
using Domain.Dto.Courier;
using Domain.Service;
using Microsoft.AspNetCore.Mvc;
using WebAPI.Helpers;

namespace WebAPI.Controllers.Courier;

[Route("v1/courier-company")]
[ApiController]
public class CourierCompanyController : BaseController
{
    private readonly ICourierCompanyService _courierCompanyService;

    public CourierCompanyController(ICourierCompanyService courierCompanyService)
    {
        _courierCompanyService = courierCompanyService;
    }

    [HttpPost("register")]
    [AuthorizeAPIRequest(true, false, AuthorizationServiceEnums.UserRoleEnums.User)]
    public async Task<ServiceObjectResult<CourierCompanyResponseDto>> Register([FromBody] RegisterCourierCompanyRequestDto requestDto)
        => await _courierCompanyService.Register(requestDto);

    [HttpGet("profile")]
    [AuthorizeAPIRequest(true, false, AuthorizationServiceEnums.UserRoleEnums.CourierCompanyAdmin)]
    public async Task<ServiceObjectResult<CourierCompanyResponseDto>> GetProfile()
        => await _courierCompanyService.GetProfile();

    [HttpPut("profile")]
    [AuthorizeAPIRequest(true, false, AuthorizationServiceEnums.UserRoleEnums.CourierCompanyAdmin)]
    public async Task<ServiceObjectResult<bool>> UpdateProfile([FromBody] RegisterCourierCompanyRequestDto requestDto)
        => await _courierCompanyService.UpdateProfile(requestDto);

    [HttpGet("members")]
    [AuthorizeAPIRequest(true, false, AuthorizationServiceEnums.UserRoleEnums.CourierCompanyAdmin)]
    public async Task<ServiceCollectionResult> GetMembers([FromQuery] int page = 1, [FromQuery] int pageSize = 20)
        => await _courierCompanyService.GetMembers(page, pageSize);

    [HttpPost("members/{courierId}")]
    [AuthorizeAPIRequest(true, false, AuthorizationServiceEnums.UserRoleEnums.CourierCompanyAdmin)]
    public async Task<ServiceObjectResult<bool>> AddMember(Guid courierId)
        => await _courierCompanyService.AddMember(courierId);

    [HttpDelete("members/{courierId}")]
    [AuthorizeAPIRequest(true, false, AuthorizationServiceEnums.UserRoleEnums.CourierCompanyAdmin)]
    public async Task<ServiceObjectResult<bool>> RemoveMember(Guid courierId)
        => await _courierCompanyService.RemoveMember(courierId);

    [HttpGet("earnings")]
    [AuthorizeAPIRequest(true, false, AuthorizationServiceEnums.UserRoleEnums.CourierCompanyAdmin)]
    public async Task<ServiceCollectionResult> GetEarnings([FromQuery] DateTime? from = null, [FromQuery] DateTime? to = null)
        => await _courierCompanyService.GetCompanyEarnings(from, to);
}
