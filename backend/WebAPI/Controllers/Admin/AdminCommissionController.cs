using Application.Services.Seller.CommissionService;
using Base.Enums;
using Domain.Dto.Seller.Commission;
using Domain.Dto.Seller.Settlement;
using Domain.Service;
using Microsoft.AspNetCore.Mvc;
using WebAPI.Helpers;

namespace WebAPI.Controllers.Admin;

[Route("v1/admin/commission")]
[ApiController]
public class AdminCommissionController : BaseController
{
    private readonly ICommissionService _commissionService;

    public AdminCommissionController(ICommissionService commissionService)
    {
        _commissionService = commissionService;
    }

    [HttpGet("settings")]
    [AuthorizeAPIRequest(true, false, UserRoleEnums.Admin)]
    public async Task<ServiceCollectionResult<PlatformCommissionScheduleDto>> GetPlatformSchedules()
    {
        return await _commissionService.GetPlatformSchedules();
    }

    [HttpGet("settings/active")]
    [AuthorizeAPIRequest(true, false, UserRoleEnums.Admin)]
    public async Task<ServiceObjectResult<PlatformCommissionScheduleDto>> GetActiveSchedule()
    {
        return await _commissionService.GetActivePlatformSchedule();
    }

    [HttpPost("settings")]
    [AuthorizeAPIRequest(true, false, UserRoleEnums.Admin)]
    public async Task<ServiceObjectResult<PlatformCommissionScheduleDto>> CreateSchedule([FromBody] CreatePlatformCommissionDto dto)
    {
        return await _commissionService.CreatePlatformSchedule(dto);
    }

    [HttpGet("restaurant/{restaurantId}")]
    [AuthorizeAPIRequest(true, false, UserRoleEnums.Admin)]
    public async Task<ServiceObjectResult<RestaurantCommissionDto>> GetRestaurantCommission(Guid restaurantId)
    {
        return await _commissionService.GetRestaurantCommission(restaurantId);
    }

    [HttpGet("restaurant/{restaurantId}/history")]
    [AuthorizeAPIRequest(true, false, UserRoleEnums.Admin)]
    public async Task<ServiceCollectionResult<RestaurantCommissionDto>> GetRestaurantCommissionHistory(Guid restaurantId)
    {
        return await _commissionService.GetRestaurantCommissionHistory(restaurantId);
    }

    [HttpPut("restaurant/{restaurantId}")]
    [AuthorizeAPIRequest(true, false, UserRoleEnums.Admin)]
    public async Task<ServiceObjectResult<RestaurantCommissionDto>> SetRestaurantCommission(Guid restaurantId, [FromBody] SetRestaurantCommissionDto dto)
    {
        return await _commissionService.SetRestaurantCommission(restaurantId, dto);
    }
}
