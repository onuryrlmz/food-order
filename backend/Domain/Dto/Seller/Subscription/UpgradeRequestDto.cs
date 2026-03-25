using Base.Entities;

namespace Domain.Dto.Seller.Subscription;

public class UpgradeRequestDto : IDto
{
    public Guid RestaurantId { get; set; }
    public Guid TargetPlanId { get; set; }
}