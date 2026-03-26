namespace Domain.Dto.Buyer.Coupon;

public class CouponCartItemDto
{
    public Guid MenuId { get; set; }
    public Guid? CategoryId { get; set; }
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
}
