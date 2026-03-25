namespace Domain.Dto.Courier;

public class EarningSummaryResponseDto
{
    public decimal TotalEarnings { get; set; }
    public decimal SettledAmount { get; set; }
    public decimal PendingAmount { get; set; }
    public int TotalDeliveries { get; set; }
    public decimal AverageEarningPerDelivery { get; set; }
}