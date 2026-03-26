namespace Domain.Dto.Seller.Subscription;

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
