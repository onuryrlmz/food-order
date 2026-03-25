namespace Domain.Dto.Courier;

public class CourierEarningResponseDto
{
    public Guid Id { get; set; }
    public Guid OrderId { get; set; }
    public decimal DeliveryFee { get; set; }
    public decimal? TipAmount { get; set; }
    public decimal? BonusAmount { get; set; }
    public decimal TotalEarning { get; set; }
    public bool IsSettled { get; set; }
    public DateTime? SettledAt { get; set; }
    public DateTime CreatedDate { get; set; }
}