namespace Domain.Dto.Seller.Subscription;

public class SubscribeRequestDto
{
    public Guid RestaurantId { get; set; }
    public Guid SubscriptionPlanId { get; set; }
}