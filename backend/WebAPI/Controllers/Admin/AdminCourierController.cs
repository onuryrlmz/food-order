using Application.Services.Admin.CourierService;
using Base.Enums;
using Domain.Dto.Admin.Courier;
using Domain.Service;
using Microsoft.AspNetCore.Mvc;
using WebAPI.Helpers;

namespace WebAPI.Controllers.Admin;

[Route("v1/admin/courier")]
[ApiController]
public class AdminCourierController : BaseController
{
    private readonly IAdminCourierService _adminCourierService;

    public AdminCourierController(IAdminCourierService adminCourierService) => _adminCourierService = adminCourierService;

    [HttpGet("list")]
    [AuthorizeAPIRequest(true, false, AuthorizationServiceEnums.UserRoleEnums.Admin)]
    public async Task<ServiceCollectionResult<AdminCourierListDto>> GetList(
        [FromQuery] int page = 1, [FromQuery] int pageSize = 20)
        => await _adminCourierService.GetCouriersAsync(page, pageSize);

    [HttpPost("approve")]
    [AuthorizeAPIRequest(true, false, AuthorizationServiceEnums.UserRoleEnums.Admin)]
    public async Task<ServiceObjectResult<bool>> Approve([FromBody] ApproveCourierDto request)
        => await _adminCourierService.ApproveCourierAsync(request);
}
