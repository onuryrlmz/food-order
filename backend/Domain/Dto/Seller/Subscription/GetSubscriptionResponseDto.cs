namespace Domain.Dto.Seller.Subscription;

public class GetSubscriptionResponseDto
{
    public Guid Id { get; set; }
    public Guid RestaurantId { get; set; }
    public string RestaurantName { get; set; }
    public Guid SubscriptionPlanId { get; set; }
    public string PlanName { get; set; }
    public decimal MonthlyPrice { get; set; }
    public short StatusId { get; set; }
    public string StatusName { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public bool IsExpired => EndDate < DateTime.UtcNow;
    public int DaysRemaining => Math.Max(0, (int)(EndDate - DateTime.UtcNow).TotalDays);
}

public class GetSubscriptionPlanResponseDto
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string? Description { get; set; }
    public short PlanType { get; set; }
    public decimal MonthlyPrice { get; set; }
    public int MaxRestaurants { get; set; }
    public int MaxOrdersPerMonth { get; set; }
}
