using Application.Services.Analytics;
using Base.Enums;
using Domain.Dto.Analytics;
using Domain.Service;
using Microsoft.AspNetCore.Mvc;
using WebAPI.Helpers;

namespace WebAPI.Controllers.Seller;

[Route("v1/seller/analytics")]
[ApiController]
public class SellerAnalyticsController : BaseController
{
    private readonly IAnalyticsService _analyticsService;

    public SellerAnalyticsController(IAnalyticsService analyticsService) => _analyticsService = analyticsService;

    [HttpGet("summary/{restaurantId}")]
    [AuthorizeAPIRequest(true, false,
        AuthorizationServiceEnums.UserRoleEnums.SellerAdmin,
        AuthorizationServiceEnums.UserRoleEnums.SellerUser)]
    public async Task<ServiceObjectResult<AnalyticsSummaryDto>> GetSummary(
        Guid restaurantId,
        [FromQuery] DateTime startDate,
        [FromQuery] DateTime endDate)
        => await _analyticsService.GetSellerSummary(restaurantId, startDate, endDate);

    [HttpGet("trends/{restaurantId}")]
    [AuthorizeAPIRequest(true, false,
        AuthorizationServiceEnums.UserRoleEnums.SellerAdmin,
        AuthorizationServiceEnums.UserRoleEnums.SellerUser)]
    public async Task<ServiceCollectionResult<OrderTrendDto>> GetOrderTrends(
        Guid restaurantId,
        [FromQuery] DateTime startDate,
        [FromQuery] DateTime endDate)
        => await _analyticsService.GetSellerOrderTrends(restaurantId, startDate, endDate);

    [HttpGet("top-products/{restaurantId}")]
    [AuthorizeAPIRequest(true, false,
        AuthorizationServiceEnums.UserRoleEnums.SellerAdmin,
        AuthorizationServiceEnums.UserRoleEnums.SellerUser)]
    public async Task<ServiceCollectionResult<TopProductDto>> GetTopProducts(
        Guid restaurantId,
        [FromQuery] DateTime startDate,
        [FromQuery] DateTime endDate,
        [FromQuery] int limit = 10)
        => await _analyticsService.GetSellerTopProducts(restaurantId, startDate, endDate, limit);
}
