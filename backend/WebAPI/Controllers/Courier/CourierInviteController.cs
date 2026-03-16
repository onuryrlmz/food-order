using Application.Services.Courier.CourierCompanyService;
using Base.Enums;
using Domain.Dto.Courier.Company;
using Domain.Service;
using Microsoft.AspNetCore.Mvc;
using WebAPI.Helpers;

namespace WebAPI.Controllers.Courier;

[Route("v1/courier")]
[ApiController]
public class CourierInviteController : BaseController
{
    private readonly ICourierCompanyService _service;

    public CourierInviteController(ICourierCompanyService service) => _service = service;

    [HttpGet("company-invites")]
    [AuthorizeAPIRequest(true, false,
        AuthorizationServiceEnums.UserRoleEnums.Courier,
        AuthorizationServiceEnums.UserRoleEnums.CourierCompanyAdmin)]
    public async Task<ServiceCollectionResult<CompanyInviteDto>> GetCompanyInvites()
        => await _service.GetCompanyInvitesAsync(Client!._tokenDto!.UserId);

    [HttpPut("company-invites/{id}/accept")]
    [AuthorizeAPIRequest(true, false,
        AuthorizationServiceEnums.UserRoleEnums.Courier,
        AuthorizationServiceEnums.UserRoleEnums.CourierCompanyAdmin)]
    public async Task<ServiceObjectResult<bool>> AcceptCompanyInvite(Guid id)
        => await _service.AcceptCompanyInviteAsync(Client!._tokenDto!.UserId, id);

    [HttpPut("company-invites/{id}/reject")]
    [AuthorizeAPIRequest(true, false,
        AuthorizationServiceEnums.UserRoleEnums.Courier,
        AuthorizationServiceEnums.UserRoleEnums.CourierCompanyAdmin)]
    public async Task<ServiceObjectResult<bool>> RejectCompanyInvite(Guid id)
        => await _service.RejectCompanyInviteAsync(Client!._tokenDto!.UserId, id);

    [HttpPut("company/leave")]
    [AuthorizeAPIRequest(true, false,
        AuthorizationServiceEnums.UserRoleEnums.Courier,
        AuthorizationServiceEnums.UserRoleEnums.CourierCompanyAdmin)]
    public async Task<ServiceObjectResult<bool>> LeaveCompany()
        => await _service.LeaveCompanyAsync(Client!._tokenDto!.UserId);
}
