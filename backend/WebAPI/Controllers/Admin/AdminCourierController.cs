using Application.Services.Courier.CourierService;
using Application.Services.Courier.CourierCompanyService;
using Application.Services.Courier.CourierEarningService;
using Base.Enums;
using Domain.Service;
using Microsoft.AspNetCore.Mvc;
using WebAPI.Helpers;

namespace WebAPI.Controllers.Admin;

[Route("v1/admin/courier")]
[ApiController]
public class AdminCourierController : BaseController
{
    private readonly ICourierService _courierService;
    private readonly ICourierCompanyService _courierCompanyService;
    private readonly ICourierEarningService _courierEarningService;

    public AdminCourierController(
        ICourierService courierService,
        ICourierCompanyService courierCompanyService,
        ICourierEarningService courierEarningService)
    {
        _courierService = courierService;
        _courierCompanyService = courierCompanyService;
        _courierEarningService = courierEarningService;
    }

    [HttpGet("companies")]
    [AuthorizeAPIRequest(true, false, AuthorizationServiceEnums.UserRoleEnums.Admin)]
    public async Task<ServiceCollectionResult> GetCompanies([FromQuery] int page = 1, [FromQuery] int pageSize = 20, [FromQuery] short? statusId = null)
    {
        return await _courierCompanyService.GetAllCompaniesForAdmin(page, pageSize, statusId);
    }

    [HttpPut("companies/{companyId}/approve")]
    [AuthorizeAPIRequest(true, false, AuthorizationServiceEnums.UserRoleEnums.Admin)]
    public async Task<ServiceObjectResult<bool>> ApproveCompany(Guid companyId)
    {
        return await _courierCompanyService.ApproveCompany(companyId);
    }

    [HttpPut("companies/{companyId}/suspend")]
    [AuthorizeAPIRequest(true, false, AuthorizationServiceEnums.UserRoleEnums.Admin)]
    public async Task<ServiceObjectResult<bool>> SuspendCompany(Guid companyId)
    {
        return await _courierCompanyService.SuspendCompany(companyId);
    }

    [HttpGet("couriers")]
    [AuthorizeAPIRequest(true, false, AuthorizationServiceEnums.UserRoleEnums.Admin)]
    public async Task<ServiceCollectionResult> GetCouriers([FromQuery] int page = 1, [FromQuery] int pageSize = 20, [FromQuery] short? statusId = null)
    {
        return await _courierService.GetAllCouriersForAdmin(page, pageSize, statusId);
    }

    [HttpPut("couriers/{courierId}/approve")]
    [AuthorizeAPIRequest(true, false, AuthorizationServiceEnums.UserRoleEnums.Admin)]
    public async Task<ServiceObjectResult<bool>> ApproveCourier(Guid courierId)
    {
        return await _courierService.ApproveCourier(courierId);
    }

    [HttpPut("couriers/{courierId}/suspend")]
    [AuthorizeAPIRequest(true, false, AuthorizationServiceEnums.UserRoleEnums.Admin)]
    public async Task<ServiceObjectResult<bool>> SuspendCourier(Guid courierId)
    {
        return await _courierService.SuspendCourier(courierId);
    }

    [HttpPost("earnings/settle")]
    [AuthorizeAPIRequest(true, false, AuthorizationServiceEnums.UserRoleEnums.Admin)]
    public async Task<ServiceObjectResult<bool>> SettleEarnings([FromBody] List<Guid> earningIds)
    {
        return await _courierEarningService.SettleEarnings(earningIds);
    }
}