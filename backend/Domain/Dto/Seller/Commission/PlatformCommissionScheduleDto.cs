namespace Domain.Dto.Seller.Commission;

public class PlatformCommissionScheduleDto
{
    public Guid Id { get; set; }
    public decimal CommissionRate { get; set; }
    public decimal FixedFee { get; set; }
    public DateTime EffectiveFrom { get; set; }
    public DateTime? EffectiveTo { get; set; }
    public string? Notes { get; set; }
    public bool IsActive { get; set; }
}
