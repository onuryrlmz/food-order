using NArchitecture.Core.Persistence.Repositories;

namespace Domain.Entities.Seller;

public class SubscriptionUsage : Entity<Guid>
{
    public Guid SubscriptionId { get; set; }
    public int Year { get; set; }
    public int Month { get; set; }
    public int OrderCount { get; set; } = 0;
    public virtual Subscription Subscription { get; set; }
}