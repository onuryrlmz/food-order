using Application.Services.Courier;
using Base.Enums;
using Domain.Dto.Courier;
using Domain.Service;
using Microsoft.AspNetCore.Mvc;
using WebAPI.Helpers;

namespace WebAPI.Controllers.Courier;

[Route("v1/courier/earnings")]
[ApiController]
public class CourierEarningsController : BaseController
{
    private readonly ICourierEarningsService _earningsService;

    public CourierEarningsController(ICourierEarningsService earningsService) => _earningsService = earningsService;

    [HttpGet("summary")]
    [AuthorizeAPIRequest(true, false,
        AuthorizationServiceEnums.UserRoleEnums.Courier,
        AuthorizationServiceEnums.UserRoleEnums.CourierCompanyAdmin)]
    public async Task<ServiceObjectResult<CourierEarningsSummaryDto>> GetSummary(
        [FromQuery] DateTime startDate,
        [FromQuery] DateTime endDate)
        => await _earningsService.GetEarningsAsync(Client!._tokenDto!.UserId, startDate, endDate);

    [HttpGet("history")]
    [AuthorizeAPIRequest(true, false,
        AuthorizationServiceEnums.UserRoleEnums.Courier,
        AuthorizationServiceEnums.UserRoleEnums.CourierCompanyAdmin)]
    public async Task<ServiceCollectionResult<CourierEarningsHistoryDto>> GetHistory(
        [FromQuery] DateTime startDate,
        [FromQuery] DateTime endDate,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20)
        => await _earningsService.GetEarningsHistoryAsync(Client!._tokenDto!.UserId, startDate, endDate, page, pageSize);

    [HttpPut("status")]
    [AuthorizeAPIRequest(true, false,
        AuthorizationServiceEnums.UserRoleEnums.Courier,
        AuthorizationServiceEnums.UserRoleEnums.CourierCompanyAdmin)]
    public async Task<ServiceObjectResult<bool>> ToggleStatus([FromQuery] short statusId)
        => await _earningsService.ToggleCourierStatusAsync(Client!._tokenDto!.UserId, statusId);
}
