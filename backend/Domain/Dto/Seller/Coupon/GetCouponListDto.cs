using Base.Enums;

namespace Domain.Dto.Seller.Coupon;

public class GetCouponListDto
{
    public Guid Id { get; set; }
    public Guid SellerId { get; set; }
    public string? SellerName { get; set; }
    public Guid? RestaurantId { get; set; }
    public string? RestaurantName { get; set; }
    public string Code { get; set; }
    public string Name { get; set; }
    public string? Description { get; set; }
    public CouponServiceEnums.CouponTypeEnums Type { get; set; }
    public decimal Value { get; set; }
    public decimal? MaxDiscountAmount { get; set; }
    public int BuyQuantity { get; set; }
    public int GetQuantity { get; set; }
    public decimal MinOrderAmount { get; set; }
    public CouponServiceEnums.CouponApplicableTypeEnums ApplicableType { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public int? UsageLimit { get; set; }
    public int? UsagePerUser { get; set; }
    public int CurrentUsageCount { get; set; }
    public DateTime CreatedDate { get; set; }
    public List<CouponApplicableItemDto> ApplicableMenus { get; set; } = new();
    public List<CouponApplicableItemDto> ApplicableCategories { get; set; } = new();
}

public class GetCouponDetailDto : GetCouponListDto
{
}

public class CouponApplicableItemDto
{
    public Guid Id { get; set; }
    public string Name { get; set; }
}