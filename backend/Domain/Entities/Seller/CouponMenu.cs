namespace Domain.Entities.Seller;

public class CouponMenu
{
    public Guid CouponId { get; set; }
    public Guid MenuId { get; set; }

    public virtual Coupon Coupon { get; set; }
    public virtual Menu Menu { get; set; }
}