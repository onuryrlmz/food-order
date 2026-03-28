using NArchitecture.Core.Persistence.Repositories;

namespace Domain.Entities.Common;

public class PlatformCommissionSchedule : Entity<Guid>
{
    public decimal CommissionRate { get; set; }
    public decimal FixedFee { get; set; }
    public DateTime EffectiveFrom { get; set; }
    public DateTime? EffectiveTo { get; set; }
    public Guid? SetByUserId { get; set; }
    public string? Notes { get; set; }
}
