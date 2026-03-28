namespace Domain.Dto.Seller.Commission;

public class RestaurantCommissionDto
{
    public Guid Id { get; set; }
    public Guid RestaurantId { get; set; }
    public string? RestaurantName { get; set; }
    public decimal CommissionRate { get; set; }
    public decimal FixedFee { get; set; }
    public DateTime EffectiveFrom { get; set; }
    public DateTime? EffectiveTo { get; set; }
    public string? Reason { get; set; }
    public string? Notes { get; set; }
    public bool IsActive { get; set; }
}
