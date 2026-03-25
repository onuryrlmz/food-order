namespace Domain.Entities.Seller;

public class CouponCategory
{
    public Guid CouponId { get; set; }
    public Guid CategoryId { get; set; }

    public virtual Coupon Coupon { get; set; }
    public virtual Category Category { get; set; }
}