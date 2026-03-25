using Domain.Entities.Buyer;
using NArchitecture.Core.Persistence.Repositories;

namespace Domain.Entities.Courier;

public class DeliveryAssignment : Entity<Guid>
{
    public Guid OrderId { get; set; }
    public Guid RestaurantId { get; set; }
    public Guid? CourierId { get; set; }
    public Guid? CourierCompanyId { get; set; }
    public Guid? AgreementId { get; set; }
    public short StatusId { get; set; }
    public short AssignmentStrategyId { get; set; }
    public decimal? DeliveryFee { get; set; }
    public decimal? DistanceKm { get; set; }
    public decimal? CustomerLatitude { get; set; }
    public decimal? CustomerLongitude { get; set; }
    public decimal? RestaurantLatitude { get; set; }
    public decimal? RestaurantLongitude { get; set; }
    public DateTime? OfferedAt { get; set; }
    public DateTime? AcceptedAt { get; set; }
    public DateTime? RejectedAt { get; set; }
    public DateTime? PickedUpAt { get; set; }
    public DateTime? DeliveredAt { get; set; }
    public DateTime? CancelledAt { get; set; }
    public string? CancellationReason { get; set; }
    public DateTime? ExpiresAt { get; set; }
    public int AttemptNumber { get; set; }
    public string? RejectionReason { get; set; }

    public virtual Order Order { get; set; }
    public virtual Courier? Courier { get; set; }
    public virtual CourierCompany? CourierCompany { get; set; }
    public virtual RestaurantCourierAgreement? Agreement { get; set; }
}