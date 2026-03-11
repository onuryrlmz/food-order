using NArchitecture.Core.Persistence.Repositories;

namespace Domain.Entities.Seller;

public class SubscriptionPlan : Entity<Guid>
{
    public string Name { get; set; }
    public string? Description { get; set; }
    public short PlanType { get; set; }
    public decimal MonthlyPrice { get; set; }
    public int MaxRestaurants { get; set; }
    public bool IsActive { get; set; } = true;

    public virtual ICollection<Subscription> Subscriptions { get; set; }
}
