using System.ComponentModel.DataAnnotations;

namespace Domain.Dto.Buyer.Coupon;

public class ValidateCouponDto
{
    [Required]
    public string Code { get; set; }

    [Required]
    public Guid RestaurantId { get; set; }

    [Required]
    [Range(0.01, double.MaxValue)]
    public decimal OrderAmount { get; set; }

    // Sepetteki ürünler
    public List<CouponCartItemDto> Items { get; set; }
}

public class CouponCartItemDto
{
    public Guid MenuId { get; set; }
    public Guid? CategoryId { get; set; }
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
}

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