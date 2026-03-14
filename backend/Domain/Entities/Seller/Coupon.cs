using Base.Enums;
using NArchitecture.Core.Persistence.Repositories;

namespace Domain.Entities.Seller;

public class Coupon : Entity<Guid>
{
    public Guid SellerId { get; set; }
    public Guid? RestaurantId { get; set; } // null ise tüm restoranlarda geçerli
    
    // Basic Info
    public string Code { get; set; } // Kupon kodu (örn: INDIRIM20)
    public string Name { get; set; }
    public string? Description { get; set; }
    
    // Type & Value
    public CouponServiceEnums.CouponTypeEnums Type { get; set; }
    public decimal Value { get; set; } // Yüzde veya tutar
    public decimal? MaxDiscountAmount { get; set; } // Maksimum indirim (yüzde için)
    
    // BuyXGetY specific
    public int BuyQuantity { get; set; } // X al Y öde için X
    public int GetQuantity { get; set; } // X al Y öde için Y
    
    // Conditions
    public decimal MinOrderAmount { get; set; } // Minimum sipariş tutarı
    public CouponServiceEnums.CouponApplicableTypeEnums ApplicableType { get; set; }
    
    // Validity
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    
    // Usage Limits
    public int? UsageLimit { get; set; } // Toplam kullanım limiti
    public int? UsagePerUser { get; set; } // Kullanıcı başına limit
    public int CurrentUsageCount { get; set; } // Şu anki kullanım sayısı
    
    // Navigation
    public virtual Seller Seller { get; set; }
    public virtual Restaurant? Restaurant { get; set; }
    public virtual ICollection<CouponMenu> CouponMenus { get; set; }
    public virtual ICollection<CouponCategory> CouponCategories { get; set; }
}
