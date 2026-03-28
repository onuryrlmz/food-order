using Application.Services.Seller.CommissionService;
using Base.Enums;
using Domain.Dto.Seller.Commission;
using Domain.Dto.Seller.Settlement;
using Domain.Service;
using Microsoft.AspNetCore.Mvc;
using WebAPI.Helpers;

namespace WebAPI.Controllers.Seller;

[Route("v1/seller/commission")]
[ApiController]
public class SellerCommissionController : BaseController
{
    private readonly ICommissionService _commissionService;

    public SellerCommissionController(ICommissionService commissionService)
    {
        _commissionService = commissionService;
    }

    [HttpGet("my")]
    [AuthorizeAPIRequest(true, false, UserRoleEnums.SellerAdmin, UserRoleEnums.SellerUser)]
    public async Task<ServiceCollectionResult<RestaurantCommissionDto>> GetMyCommissions()
    {
        return await _commissionService.GetMyCommissions();
    }

    [HttpGet("settlement/periods")]
    [AuthorizeAPIRequest(true, false, UserRoleEnums.SellerAdmin, UserRoleEnums.SellerUser)]
    public async Task<ServiceCollectionResult<SettlementPeriodDto>> GetMySettlementPeriods(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20)
    {
        return await _commissionService.GetMySettlementPeriods(page, pageSize);
    }

    [HttpGet("settlement/period/{periodId}")]
    [AuthorizeAPIRequest(true, false, UserRoleEnums.SellerAdmin, UserRoleEnums.SellerUser)]
    public async Task<ServiceObjectResult<SettlementPeriodDetailDto>> GetMySettlementPeriodDetail(Guid periodId)
    {
        return await _commissionService.GetMySettlementPeriodDetail(periodId);
    }
}
