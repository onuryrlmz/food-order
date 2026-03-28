namespace Domain.Dto.Seller.Commission;

public class SetRestaurantCommissionDto
{
    public decimal CommissionRate { get; set; }
    public decimal FixedFee { get; set; }
    public DateTime? EffectiveFrom { get; set; }
    public string? Reason { get; set; }
    public string? Notes { get; set; }
}
