using NArchitecture.Core.Persistence.Repositories;

namespace Domain.Entities.Courier;

public class CourierEarning : Entity<Guid>
{
    public Guid CourierId { get; set; }
    public Guid DeliveryAssignmentId { get; set; }
    public Guid OrderId { get; set; }
    public decimal DeliveryFee { get; set; }
    public decimal? TipAmount { get; set; }
    public decimal? BonusAmount { get; set; }
    public decimal TotalEarning { get; set; }
    public bool IsSettled { get; set; }
    public DateTime? SettledAt { get; set; }
    public string? SettlementReference { get; set; }

    public virtual Courier Courier { get; set; }
    public virtual DeliveryAssignment DeliveryAssignment { get; set; }
}