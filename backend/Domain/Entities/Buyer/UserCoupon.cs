using Domain.Entities.Common;
using NArchitecture.Core.Persistence.Repositories;

namespace Domain.Entities.Buyer;

public class UserCoupon : Entity<Guid>
{
    public Guid UserId { get; set; }
    public Guid CouponId { get; set; }
    public Guid? OrderId { get; set; } // Kullanıldığı sipariş
    public DateTime? UsedAt { get; set; }
    public int UsageCount { get; set; } // Kaç kez kullandı
    
    public virtual User User { get; set; }
    public virtual Seller.Coupon Coupon { get; set; }
    public virtual Order? Order { get; set; }
}
