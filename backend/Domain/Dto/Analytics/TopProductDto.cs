namespace Domain.Dto.Analytics;

public class TopProductDto
{
    public Guid MenuId { get; set; }
    public string MenuName { get; set; }
    public int OrderCount { get; set; }
    public decimal TotalRevenue { get; set; }
}
