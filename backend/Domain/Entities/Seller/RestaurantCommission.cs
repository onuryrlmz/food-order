using NArchitecture.Core.Persistence.Repositories;

namespace Domain.Entities.Seller;

public class RestaurantCommission : Entity<Guid>
{
    public Guid RestaurantId { get; set; }
    public decimal CommissionRate { get; set; }
    public decimal FixedFee { get; set; }
    public DateTime EffectiveFrom { get; set; }
    public DateTime? EffectiveTo { get; set; }
    public Guid? SetByUserId { get; set; }
    public string? Reason { get; set; }
    public string? Notes { get; set; }
    public virtual Restaurant Restaurant { get; set; }
}
