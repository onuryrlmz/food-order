namespace Domain.Dto.Analytics;

public class AnalyticsSummaryDto
{
    public int TotalOrders { get; set; }
    public decimal TotalRevenue { get; set; }
    public decimal AverageOrderValue { get; set; }
    public int UniqueCustomers { get; set; }
}

public class OrderTrendDto
{
    public string Date { get; set; }
    public int OrderCount { get; set; }
    public decimal Revenue { get; set; }
}

public class TopProductDto
{
    public Guid MenuId { get; set; }
    public string MenuName { get; set; }
    public int OrderCount { get; set; }
    public decimal TotalRevenue { get; set; }
}

public class TopRestaurantDto
{
    public Guid RestaurantId { get; set; }
    public string RestaurantName { get; set; }
    public int OrderCount { get; set; }
    public decimal TotalRevenue { get; set; }
}