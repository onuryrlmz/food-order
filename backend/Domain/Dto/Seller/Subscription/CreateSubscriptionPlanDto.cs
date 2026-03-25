namespace Domain.Dto.Seller.Subscription;

public class CreateSubscriptionPlanDto
{
    public string Name { get; set; }
    public string? Description { get; set; }
    public short PlanType { get; set; }
    public decimal MonthlyPrice { get; set; }
    public int MaxRestaurants { get; set; }
}