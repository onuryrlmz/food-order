using Application.Services.Analytics;
using Base.Enums;
using Domain.Dto.Analytics;
using Domain.Service;
using Microsoft.AspNetCore.Mvc;
using WebAPI.Helpers;

namespace WebAPI.Controllers.Admin;

[Route("v1/admin/analytics")]
[ApiController]
public class AdminAnalyticsController : BaseController
{
    private readonly IAnalyticsService _analyticsService;

    public AdminAnalyticsController(IAnalyticsService analyticsService)
    {
        _analyticsService = analyticsService;
    }

    [HttpGet("summary")]
    [AuthorizeAPIRequest(true, false, UserRoleEnums.Admin)]
    public async Task<ServiceObjectResult<AnalyticsSummaryDto>> GetSummary(
        [FromQuery] DateTime startDate,
        [FromQuery] DateTime endDate)
    {
        return await _analyticsService.GetAdminSummary(startDate, endDate);
    }

    [HttpGet("trends")]
    [AuthorizeAPIRequest(true, false, UserRoleEnums.Admin)]
    public async Task<ServiceCollectionResult<OrderTrendDto>> GetOrderTrends(
        [FromQuery] DateTime startDate,
        [FromQuery] DateTime endDate)
    {
        return await _analyticsService.GetAdminOrderTrends(startDate, endDate);
    }

    [HttpGet("top-restaurants")]
    [AuthorizeAPIRequest(true, false, UserRoleEnums.Admin)]
    public async Task<ServiceCollectionResult<TopRestaurantDto>> GetTopRestaurants(
        [FromQuery] DateTime startDate,
        [FromQuery] DateTime endDate,
        [FromQuery] int limit = 10)
    {
        return await _analyticsService.GetAdminTopRestaurants(startDate, endDate, limit);
    }
}