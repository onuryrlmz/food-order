using Base.Entities;

namespace Domain.Dto.Seller.Subscription;

public class SubscriptionUsageDto : IDto
{
    public int OrderCount { get; set; }
    public int MaxOrdersPerMonth { get; set; }
    public double UsagePercentage { get; set; }
    public int RemainingDays { get; set; }
    public string PlanName { get; set; }
}
