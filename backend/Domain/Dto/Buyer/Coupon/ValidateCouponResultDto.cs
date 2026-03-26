namespace Domain.Dto.Buyer.Coupon;

public class ValidateCouponResultDto
{
    public bool IsValid { get; set; }
    public string? ErrorMessage { get; set; }
    public Guid CouponId { get; set; }
    public string CouponName { get; set; }
    public string CouponCode { get; set; }
    public decimal DiscountAmount { get; set; }
    public decimal OriginalAmount { get; set; }
    public decimal FinalAmount { get; set; }
    public string DiscountDescription { get; set; }
}
