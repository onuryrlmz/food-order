namespace Domain.Dto.Analytics;

public class TopRestaurantDto
{
    public Guid RestaurantId { get; set; }
    public string RestaurantName { get; set; }
    public int OrderCount { get; set; }
    public decimal TotalRevenue { get; set; }
}
