using NArchitecture.Core.Persistence.Repositories;

namespace Domain.Entities.Seller;

public class SettlementItem : Entity<Guid>
{
    public Guid OrderId { get; set; }
    public Guid SellerId { get; set; }
    public Guid RestaurantId { get; set; }
    public decimal OrderAmount { get; set; }
    public decimal CommissionRate { get; set; }
    public decimal CommissionAmount { get; set; }
    public decimal FixedFee { get; set; }
    public decimal NetAmount { get; set; }
    public short CommissionSourceType { get; set; }
    public Guid CommissionSourceId { get; set; }
    public DateTime PeriodDate { get; set; }
    public Guid? SettlementPeriodId { get; set; }
    public string? Notes { get; set; }
    public virtual SettlementPeriod? SettlementPeriod { get; set; }
}
