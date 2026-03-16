using System.Data;
using Application.Services.Common.TokenService;
using Dapper;
using Domain.Dto.Analytics;
using Domain.Service;
using Microsoft.EntityFrameworkCore;
using Persistence.Contexts;

namespace Application.Services.Analytics;

public class AnalyticsManager : IAnalyticsService
{
    private readonly BaseDbContext _context;
    private readonly ITokenAccessor _tokenAccessor;

    public AnalyticsManager(BaseDbContext context, ITokenAccessor tokenAccessor)
    {
        _context = context;
        _tokenAccessor = tokenAccessor;
    }

    public async Task<ServiceObjectResult<AnalyticsSummaryDto>> GetSellerSummary(Guid restaurantId, DateTime startDate, DateTime endDate)
    {
        var result = new ServiceObjectResult<AnalyticsSummaryDto>();
        try
        {
            const string query = @"
                SELECT
                    COUNT(*) AS TotalOrders,
                    COALESCE(SUM(o.TotalPrice), 0) AS TotalRevenue,
                    COALESCE(AVG(o.TotalPrice), 0) AS AverageOrderValue,
                    COUNT(DISTINCT o.UserId) AS UniqueCustomers
                FROM `Order` o
                WHERE o.RestaurantId = @RestaurantId
                    AND o.StatusId = 8
                    AND o.CreatedDate >= @StartDate
                    AND o.CreatedDate <= @EndDate
                    AND o.DeletedDate IS NULL
            ";

            var conn = _context.Database.GetDbConnection();
            if (conn.State != ConnectionState.Open)
                await conn.OpenAsync();

            var summary = await conn.QueryFirstOrDefaultAsync<AnalyticsSummaryDto>(
                query,
                new { RestaurantId = restaurantId.ToString(), StartDate = startDate, EndDate = endDate });

            result.SetData(summary ?? new AnalyticsSummaryDto());
        }
        catch (Exception e)
        {
            result.Fail(e);
        }
        return result;
    }

    public async Task<ServiceCollectionResult<OrderTrendDto>> GetSellerOrderTrends(Guid restaurantId, DateTime startDate, DateTime endDate)
    {
        var result = new ServiceCollectionResult<OrderTrendDto>();
        try
        {
            const string query = @"
                SELECT
                    DATE_FORMAT(o.CreatedDate, '%Y-%m-%d') AS `Date`,
                    COUNT(*) AS OrderCount,
                    COALESCE(SUM(o.TotalPrice), 0) AS Revenue
                FROM `Order` o
                WHERE o.RestaurantId = @RestaurantId
                    AND o.StatusId = 8
                    AND o.CreatedDate >= @StartDate
                    AND o.CreatedDate <= @EndDate
                    AND o.DeletedDate IS NULL
                GROUP BY DATE_FORMAT(o.CreatedDate, '%Y-%m-%d')
                ORDER BY `Date` ASC
            ";

            var conn = _context.Database.GetDbConnection();
            if (conn.State != ConnectionState.Open)
                await conn.OpenAsync();

            var trends = (await conn.QueryAsync<OrderTrendDto>(
                query,
                new { RestaurantId = restaurantId.ToString(), StartDate = startDate, EndDate = endDate })).ToList();

            result.SetData(trends);
        }
        catch (Exception e)
        {
            result.Fail(e);
        }
        return result;
    }

    public async Task<ServiceCollectionResult<TopProductDto>> GetSellerTopProducts(Guid restaurantId, DateTime startDate, DateTime endDate, int limit = 10)
    {
        var result = new ServiceCollectionResult<TopProductDto>();
        try
        {
            const string query = @"
                SELECT
                    oi.MenuId,
                    m.Name AS MenuName,
                    SUM(oi.Quantity) AS OrderCount,
                    SUM(oi.TotalPrice) AS TotalRevenue
                FROM `OrderItem` oi
                INNER JOIN `Order` o ON o.Id = oi.OrderId
                LEFT JOIN `Menu` m ON m.Id = oi.MenuId
                WHERE o.RestaurantId = @RestaurantId
                    AND o.StatusId = 8
                    AND o.CreatedDate >= @StartDate
                    AND o.CreatedDate <= @EndDate
                    AND o.DeletedDate IS NULL
                GROUP BY oi.MenuId, m.Name
                ORDER BY TotalRevenue DESC
                LIMIT @Limit
            ";

            var conn = _context.Database.GetDbConnection();
            if (conn.State != ConnectionState.Open)
                await conn.OpenAsync();

            var products = (await conn.QueryAsync<TopProductDto>(
                query,
                new { RestaurantId = restaurantId.ToString(), StartDate = startDate, EndDate = endDate, Limit = limit })).ToList();

            result.SetData(products);
        }
        catch (Exception e)
        {
            result.Fail(e);
        }
        return result;
    }

    public async Task<ServiceObjectResult<AnalyticsSummaryDto>> GetAdminSummary(DateTime startDate, DateTime endDate)
    {
        var result = new ServiceObjectResult<AnalyticsSummaryDto>();
        try
        {
            const string query = @"
                SELECT
                    COUNT(*) AS TotalOrders,
                    COALESCE(SUM(o.TotalPrice), 0) AS TotalRevenue,
                    COALESCE(AVG(o.TotalPrice), 0) AS AverageOrderValue,
                    COUNT(DISTINCT o.UserId) AS UniqueCustomers
                FROM `Order` o
                WHERE o.StatusId = 8
                    AND o.CreatedDate >= @StartDate
                    AND o.CreatedDate <= @EndDate
                    AND o.DeletedDate IS NULL
            ";

            var conn = _context.Database.GetDbConnection();
            if (conn.State != ConnectionState.Open)
                await conn.OpenAsync();

            var summary = await conn.QueryFirstOrDefaultAsync<AnalyticsSummaryDto>(
                query,
                new { StartDate = startDate, EndDate = endDate });

            result.SetData(summary ?? new AnalyticsSummaryDto());
        }
        catch (Exception e)
        {
            result.Fail(e);
        }
        return result;
    }

    public async Task<ServiceCollectionResult<OrderTrendDto>> GetAdminOrderTrends(DateTime startDate, DateTime endDate)
    {
        var result = new ServiceCollectionResult<OrderTrendDto>();
        try
        {
            const string query = @"
                SELECT
                    DATE_FORMAT(o.CreatedDate, '%Y-%m-%d') AS `Date`,
                    COUNT(*) AS OrderCount,
                    COALESCE(SUM(o.TotalPrice), 0) AS Revenue
                FROM `Order` o
                WHERE o.StatusId = 8
                    AND o.CreatedDate >= @StartDate
                    AND o.CreatedDate <= @EndDate
                    AND o.DeletedDate IS NULL
                GROUP BY DATE_FORMAT(o.CreatedDate, '%Y-%m-%d')
                ORDER BY `Date` ASC
            ";

            var conn = _context.Database.GetDbConnection();
            if (conn.State != ConnectionState.Open)
                await conn.OpenAsync();

            var trends = (await conn.QueryAsync<OrderTrendDto>(
                query,
                new { StartDate = startDate, EndDate = endDate })).ToList();

            result.SetData(trends);
        }
        catch (Exception e)
        {
            result.Fail(e);
        }
        return result;
    }

    public async Task<ServiceCollectionResult<TopRestaurantDto>> GetAdminTopRestaurants(DateTime startDate, DateTime endDate, int limit = 10)
    {
        var result = new ServiceCollectionResult<TopRestaurantDto>();
        try
        {
            const string query = @"
                SELECT
                    o.RestaurantId,
                    r.Name AS RestaurantName,
                    COUNT(*) AS OrderCount,
                    SUM(o.TotalPrice) AS TotalRevenue
                FROM `Order` o
                INNER JOIN `Restaurant` r ON r.Id = o.RestaurantId
                WHERE o.StatusId = 8
                    AND o.CreatedDate >= @StartDate
                    AND o.CreatedDate <= @EndDate
                    AND o.DeletedDate IS NULL
                GROUP BY o.RestaurantId, r.Name
                ORDER BY TotalRevenue DESC
                LIMIT @Limit
            ";

            var conn = _context.Database.GetDbConnection();
            if (conn.State != ConnectionState.Open)
                await conn.OpenAsync();

            var restaurants = (await conn.QueryAsync<TopRestaurantDto>(
                query,
                new { StartDate = startDate, EndDate = endDate, Limit = limit })).ToList();

            result.SetData(restaurants);
        }
        catch (Exception e)
        {
            result.Fail(e);
        }
        return result;
    }
}
