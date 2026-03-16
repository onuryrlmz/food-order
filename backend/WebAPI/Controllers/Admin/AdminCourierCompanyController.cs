using Application.Services.Courier.CourierCompanyService;
using Base.Enums;
using Domain.Dto.Courier.Company;
using Domain.Service;
using Microsoft.AspNetCore.Mvc;
using WebAPI.Helpers;

namespace WebAPI.Controllers.Admin;

[Route("v1/admin/courier-companies")]
[ApiController]
public class AdminCourierCompanyController : BaseController
{
    private readonly ICourierCompanyService _service;

    public AdminCourierCompanyController(ICourierCompanyService service) => _service = service;

    [HttpGet]
    [AuthorizeAPIRequest(true, false, AuthorizationServiceEnums.UserRoleEnums.Admin)]
    public async Task<ServiceCollectionResult<CourierCompanyDto>> GetAll(
        [FromQuery] int page = 1, [FromQuery] int size = 20)
        => await _service.GetAllCompaniesAsync(page, size);

    [HttpPut("{id}/approve")]
    [AuthorizeAPIRequest(true, false, AuthorizationServiceEnums.UserRoleEnums.Admin)]
    public async Task<ServiceObjectResult<bool>> Approve(Guid id)
        => await _service.ApproveCompanyAsync(id);

    [HttpPut("{id}/reject")]
    [AuthorizeAPIRequest(true, false, AuthorizationServiceEnums.UserRoleEnums.Admin)]
    public async Task<ServiceObjectResult<bool>> Reject(Guid id)
        => await _service.RejectCompanyAsync(id);

    [HttpPut("{id}/suspend")]
    [AuthorizeAPIRequest(true, false, AuthorizationServiceEnums.UserRoleEnums.Admin)]
    public async Task<ServiceObjectResult<bool>> Suspend(Guid id)
        => await _service.SuspendCompanyAsync(id);

    [HttpPut("{id}/ban")]
    [AuthorizeAPIRequest(true, false, AuthorizationServiceEnums.UserRoleEnums.Admin)]
    public async Task<ServiceObjectResult<bool>> Ban(Guid id)
        => await _service.BanCompanyAsync(id);
}
