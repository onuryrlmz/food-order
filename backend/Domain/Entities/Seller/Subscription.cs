using NArchitecture.Core.Persistence.Repositories;

namespace Domain.Entities.Seller;

public class Subscription : Entity<Guid>
{
    public Guid SellerId { get; set; }
    public Guid RestaurantId { get; set; }
    public Guid SubscriptionPlanId { get; set; }
    public short StatusId { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public decimal PaidAmount { get; set; }
    public string? Notes { get; set; }

    public virtual Seller Seller { get; set; }
    public virtual Restaurant Restaurant { get; set; }
    public virtual SubscriptionPlan SubscriptionPlan { get; set; }
}
