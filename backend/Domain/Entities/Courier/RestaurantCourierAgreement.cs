using NArchitecture.Core.Persistence.Repositories;

namespace Domain.Entities.Courier;

public class RestaurantCourierAgreement : Entity<Guid>
{
    public Guid RestaurantId { get; set; }
    public Guid? CourierCompanyId { get; set; }
    public Guid? CourierId { get; set; }
    public short StatusId { get; set; }
    public short AssignmentStrategyId { get; set; }
    public decimal? AgreedDeliveryFee { get; set; }
    public decimal? PerKmFee { get; set; }
    public int Priority { get; set; }
    public bool IsDefault { get; set; }
    public DateTime? EffectiveFrom { get; set; }
    public DateTime? EffectiveUntil { get; set; }

    public virtual Seller.Restaurant Restaurant { get; set; }
    public virtual CourierCompany? CourierCompany { get; set; }
    public virtual Courier? Courier { get; set; }
}
