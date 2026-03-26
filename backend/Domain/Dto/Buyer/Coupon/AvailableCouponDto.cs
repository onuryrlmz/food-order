namespace Domain.Dto.Buyer.Coupon;

public class AvailableCouponDto
{
    public Guid Id { get; set; }
    public string Code { get; set; }
    public string Name { get; set; }
    public string? Description { get; set; }
    public string RestaurantName { get; set; }
    public decimal MinOrderAmount { get; set; }
    public decimal? DiscountPreview { get; set; }
    public DateTime EndDate { get; set; }
}
