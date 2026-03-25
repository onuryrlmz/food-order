using Base.Enums;
using System.ComponentModel.DataAnnotations;

namespace Domain.Dto.Seller.Coupon;

public class UpdateCouponDto
{
    [Required]
    public Guid Id { get; set; }

    [Required]
    public Guid? RestaurantId { get; set; }

    [Required]
    [StringLength(50, MinimumLength = 3)]
    public string Code { get; set; }

    [Required]
    [StringLength(200, MinimumLength = 2)]
    public string Name { get; set; }

    [StringLength(500)]
    public string? Description { get; set; }

    [Required]
    public CouponServiceEnums.CouponTypeEnums Type { get; set; }

    [Required]
    [Range(0.01, double.MaxValue)]
    public decimal Value { get; set; }

    [Range(0.01, double.MaxValue)]
    public decimal? MaxDiscountAmount { get; set; }

    [Range(1, int.MaxValue)]
    public int BuyQuantity { get; set; } = 2;

    [Range(1, int.MaxValue)]
    public int GetQuantity { get; set; } = 1;

    [Required]
    [Range(0, double.MaxValue)]
    public decimal MinOrderAmount { get; set; }

    [Required]
    public CouponServiceEnums.CouponApplicableTypeEnums ApplicableType { get; set; }

    [Required]
    public DateTime StartDate { get; set; }

    [Required]
    public DateTime EndDate { get; set; }

    [Range(1, int.MaxValue)]
    public int? UsageLimit { get; set; }

    [Range(1, int.MaxValue)]
    public int? UsagePerUser { get; set; }

    public List<Guid>? ApplicableMenuIds { get; set; }
    public List<Guid>? ApplicableCategoryIds { get; set; }
}