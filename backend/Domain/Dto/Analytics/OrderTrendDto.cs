namespace Domain.Dto.Analytics;

public class OrderTrendDto
{
    public string Date { get; set; }
    public int OrderCount { get; set; }
    public decimal Revenue { get; set; }
}
