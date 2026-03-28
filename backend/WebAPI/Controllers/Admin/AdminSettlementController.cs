using Application.Services.Seller.CommissionService;
using Base.Enums;
using Domain.Dto.Seller.Settlement;
using Domain.Service;
using Microsoft.AspNetCore.Mvc;
using WebAPI.Helpers;

namespace WebAPI.Controllers.Admin;

[Route("v1/admin/settlement")]
[ApiController]
public class AdminSettlementController : BaseController
{
    private readonly ICommissionService _commissionService;

    public AdminSettlementController(ICommissionService commissionService)
    {
        _commissionService = commissionService;
    }

    [HttpGet("periods")]
    [AuthorizeAPIRequest(true, false, UserRoleEnums.Admin)]
    public async Task<ServiceCollectionResult<SettlementPeriodDto>> GetPeriods(
        [FromQuery] short? statusId,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20)
    {
        return await _commissionService.GetSettlementPeriods(statusId, page, pageSize);
    }

    [HttpGet("period/{periodId}")]
    [AuthorizeAPIRequest(true, false, UserRoleEnums.Admin)]
    public async Task<ServiceObjectResult<SettlementPeriodDetailDto>> GetPeriodDetail(Guid periodId)
    {
        return await _commissionService.GetSettlementPeriodDetail(periodId);
    }

    [HttpPost("period/{periodId}/approve")]
    [AuthorizeAPIRequest(true, false, UserRoleEnums.Admin)]
    public async Task<ServiceObjectResult<bool>> Approve(Guid periodId)
    {
        return await _commissionService.ApproveSettlement(periodId);
    }

    [HttpPost("period/{periodId}/pay")]
    [AuthorizeAPIRequest(true, false, UserRoleEnums.Admin)]
    public async Task<ServiceObjectResult<bool>> Pay(Guid periodId, [FromBody] PaySettlementDto dto)
    {
        return await _commissionService.PaySettlement(periodId, dto);
    }

    [HttpPost("period/{periodId}/cancel")]
    [AuthorizeAPIRequest(true, false, UserRoleEnums.Admin)]
    public async Task<ServiceObjectResult<bool>> Cancel(Guid periodId, [FromQuery] string? reason)
    {
        return await _commissionService.CancelSettlement(periodId, reason);
    }
}
