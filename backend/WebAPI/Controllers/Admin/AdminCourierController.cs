using Application.Services.Courier.CourierService;
using Application.Services.Courier.CourierCompanyService;
using Application.Services.Courier.CourierEarningService;
using Application.Services.Courier.DeliveryAssignmentService;
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
    private readonly IDeliveryAssignmentService _deliveryAssignmentService;

    public AdminCourierController(
        ICourierService courierService,
        ICourierCompanyService courierCompanyService,
        ICourierEarningService courierEarningService,
        IDeliveryAssignmentService deliveryAssignmentService)
    {
        _courierService = courierService;
        _courierCompanyService = courierCompanyService;
        _courierEarningService = courierEarningService;
        _deliveryAssignmentService = deliveryAssignmentService;
    }

    [HttpGet("companies")]
    [AuthorizeAPIRequest(true, false, UserRoleEnums.Admin)]
    public async Task<ServiceCollectionResult> GetCompanies([FromQuery] int page = 1, [FromQuery] int pageSize = 20, [FromQuery] short? statusId = null)
    {
        return await _courierCompanyService.GetAllCompaniesForAdmin(page, pageSize, statusId);
    }

    [HttpPut("companies/{companyId}/approve")]
    [AuthorizeAPIRequest(true, false, UserRoleEnums.Admin)]
    public async Task<ServiceObjectResult<bool>> ApproveCompany(Guid companyId)
    {
        return await _courierCompanyService.ApproveCompany(companyId);
    }

    [HttpPut("companies/{companyId}/suspend")]
    [AuthorizeAPIRequest(true, false, UserRoleEnums.Admin)]
    public async Task<ServiceObjectResult<bool>> SuspendCompany(Guid companyId)
    {
        return await _courierCompanyService.SuspendCompany(companyId);
    }

    [HttpGet("couriers")]
    [AuthorizeAPIRequest(true, false, UserRoleEnums.Admin)]
    public async Task<ServiceCollectionResult> GetCouriers([FromQuery] int page = 1, [FromQuery] int pageSize = 20, [FromQuery] short? statusId = null)
    {
        return await _courierService.GetAllCouriersForAdmin(page, pageSize, statusId);
    }

    [HttpPut("couriers/{courierId}/approve")]
    [AuthorizeAPIRequest(true, false, UserRoleEnums.Admin)]
    public async Task<ServiceObjectResult<bool>> ApproveCourier(Guid courierId)
    {
        return await _courierService.ApproveCourier(courierId);
    }

    [HttpPut("couriers/{courierId}/suspend")]
    [AuthorizeAPIRequest(true, false, UserRoleEnums.Admin)]
    public async Task<ServiceObjectResult<bool>> SuspendCourier(Guid courierId)
    {
        return await _courierService.SuspendCourier(courierId);
    }

    [HttpPost("earnings/settle")]
    [AuthorizeAPIRequest(true, false, UserRoleEnums.Admin)]
    public async Task<ServiceObjectResult<bool>> SettleEarnings([FromBody] List<Guid> earningIds)
    {
        return await _courierEarningService.SettleEarnings(earningIds);
    }

    [HttpPost("assignment/{assignmentId}/cancel")]
    [AuthorizeAPIRequest(true, false, UserRoleEnums.Admin)]
    public async Task<ServiceObjectResult<bool>> CancelAssignment(Guid assignmentId, [FromQuery] string reason = "Admin tarafından iptal edildi")
    {
        return await _deliveryAssignmentService.CancelAssignment(assignmentId, reason);
    }
}