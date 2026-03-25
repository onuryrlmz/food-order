using Domain.Dto.Analytics;
using Domain.Service;

namespace Application.Services.Analytics;

public interface IAnalyticsService
{
    // Seller analytics
    Task<ServiceObjectResult<AnalyticsSummaryDto>> GetSellerSummary(Guid restaurantId, DateTime startDate, DateTime endDate);
    Task<ServiceCollectionResult<OrderTrendDto>> GetSellerOrderTrends(Guid restaurantId, DateTime startDate, DateTime endDate);
    Task<ServiceCollectionResult<TopProductDto>> GetSellerTopProducts(Guid restaurantId, DateTime startDate, DateTime endDate, int limit = 10);

    // Admin analytics
    Task<ServiceObjectResult<AnalyticsSummaryDto>> GetAdminSummary(DateTime startDate, DateTime endDate);
    Task<ServiceCollectionResult<OrderTrendDto>> GetAdminOrderTrends(DateTime startDate, DateTime endDate);
    Task<ServiceCollectionResult<TopRestaurantDto>> GetAdminTopRestaurants(DateTime startDate, DateTime endDate, int limit = 10);
}