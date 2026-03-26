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
