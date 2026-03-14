using Base.Enums;
using Domain.Dto.Buyer.Coupon;
using Domain.Entities.Seller;
using Domain.Service;
using Microsoft.EntityFrameworkCore;
using Persistence.Contexts;
using Application.Services.Common.TokenService;

namespace Application.Services.Buyer.CouponService;

public interface ICouponValidationService
{
    Task<ServiceObjectResult<ValidateCouponResultDto>> ValidateCoupon(ValidateCouponDto requestDto);
    Task<ServiceCollectionResult<AvailableCouponDto>> GetAvailableCoupons(Guid restaurantId, decimal orderAmount);
    Task<ServiceObjectResult<bool>> ApplyCouponToOrder(Guid orderId, Guid couponId);
}

public class CouponValidationManager : ICouponValidationService
{
    private readonly BaseDbContext _context;
    private readonly ITokenAccessor _tokenAccessor;

    public CouponValidationManager(BaseDbContext context, ITokenAccessor tokenAccessor)
    {
        _context = context;
        _tokenAccessor = tokenAccessor;
    }

    public async Task<ServiceObjectResult<ValidateCouponResultDto>> ValidateCoupon(ValidateCouponDto requestDto)
    {
        var result = new ServiceObjectResult<ValidateCouponResultDto>();
        try
        {
            var token = _tokenAccessor.GetToken();
            if (token == null)
            {
                result.Fail("User not authenticated");
                return result;
            }
            var userId = token.UserId;

            var coupon = await _context.Coupons
                .Include(c => c.CouponMenus)
                .Include(c => c.CouponCategories)
                .FirstOrDefaultAsync(c => c.Code.ToLower() == requestDto.Code.ToLower() && c.DeletedDate == null);

            if (coupon == null)
            {
                result.SetData(new ValidateCouponResultDto
                {
                    IsValid = false,
                    ErrorMessage = "Kupon kodu bulunamadı"
                });
                return result;
            }

            // Validate date range
            var now = DateTime.UtcNow;
            if (now < coupon.StartDate || now > coupon.EndDate)
            {
                result.SetData(new ValidateCouponResultDto
                {
                    IsValid = false,
                    ErrorMessage = "Kupon süresi dolmuş veya henüz başlamamış"
                });
                return result;
            }

            // Validate restaurant
            if (coupon.RestaurantId.HasValue && coupon.RestaurantId != requestDto.RestaurantId)
            {
                result.SetData(new ValidateCouponResultDto
                {
                    IsValid = false,
                    ErrorMessage = "Bu kupon bu restoranda geçerli değil"
                });
                return result;
            }

            // Validate minimum order amount
            if (requestDto.OrderAmount < coupon.MinOrderAmount)
            {
                result.SetData(new ValidateCouponResultDto
                {
                    IsValid = false,
                    ErrorMessage = $"Minimum sipariş tutarı ₺{coupon.MinOrderAmount:F2} olmalıdır"
                });
                return result;
            }

            // Validate usage limit
            if (coupon.UsageLimit.HasValue && coupon.CurrentUsageCount >= coupon.UsageLimit.Value)
            {
                result.SetData(new ValidateCouponResultDto
                {
                    IsValid = false,
                    ErrorMessage = "Bu kuponun kullanım limiti dolmuş"
                });
                return result;
            }

            // Validate user usage limit
            if (coupon.UsagePerUser.HasValue)
            {
                var userCoupon = await _context.UserCoupons
                    .FirstOrDefaultAsync(uc => uc.UserId == userId && uc.CouponId == coupon.Id);

                if (userCoupon != null && userCoupon.UsageCount >= coupon.UsagePerUser.Value)
                {
                    result.SetData(new ValidateCouponResultDto
                    {
                        IsValid = false,
                        ErrorMessage = $"Bu kupondan en fazla {coupon.UsagePerUser.Value} kez kullanabilirsiniz"
                    });
                    return result;
                }
            }

            // Calculate discount
            var discountAmount = CalculateDiscount(coupon, requestDto.Items, requestDto.OrderAmount);

            result.SetData(new ValidateCouponResultDto
            {
                IsValid = true,
                CouponId = coupon.Id,
                CouponName = coupon.Name,
                CouponCode = coupon.Code,
                DiscountAmount = discountAmount,
                OriginalAmount = requestDto.OrderAmount,
                FinalAmount = requestDto.OrderAmount - discountAmount,
                DiscountDescription = GetDiscountDescription(coupon, discountAmount)
            });
        }
        catch (Exception e)
        {
            result.Fail(e);
        }

        return result;
    }

    public async Task<ServiceCollectionResult<AvailableCouponDto>> GetAvailableCoupons(Guid restaurantId, decimal orderAmount)
    {
        var result = new ServiceCollectionResult<AvailableCouponDto>();
        try
        {
            var token = _tokenAccessor.GetToken();
            if (token == null)
            {
                result.Fail("User not authenticated");
                return result;
            }
            var userId = token.UserId;

            var now = DateTime.UtcNow;

            var coupons = await _context.Coupons
                .Include(c => c.Restaurant)
                .Where(c => c.DeletedDate == null
                    && c.StartDate <= now
                    && c.EndDate >= now
                    && (c.RestaurantId == null || c.RestaurantId == restaurantId)
                    && c.MinOrderAmount <= orderAmount
                    && (c.UsageLimit == null || c.CurrentUsageCount < c.UsageLimit))
                .ToListAsync();

            var availableCoupons = new List<AvailableCouponDto>();

            foreach (var coupon in coupons)
            {
                // Check user usage limit
                if (coupon.UsagePerUser.HasValue)
                {
                    var userCoupon = await _context.UserCoupons
                        .FirstOrDefaultAsync(uc => uc.UserId == userId && uc.CouponId == coupon.Id);

                    if (userCoupon != null && userCoupon.UsageCount >= coupon.UsagePerUser.Value)
                        continue;
                }

                var discountPreview = CalculateDiscountPreview(coupon, orderAmount);

                availableCoupons.Add(new AvailableCouponDto
                {
                    Id = coupon.Id,
                    Code = coupon.Code,
                    Name = coupon.Name,
                    Description = coupon.Description,
                    RestaurantName = coupon.Restaurant?.Name ?? "Tüm Restoranlar",
                    MinOrderAmount = coupon.MinOrderAmount,
                    DiscountPreview = discountPreview,
                    EndDate = coupon.EndDate
                });
            }

            result.SetData(availableCoupons);
        }
        catch (Exception e)
        {
            result.Fail(e);
        }

        return result;
    }

    public async Task<ServiceObjectResult<bool>> ApplyCouponToOrder(Guid orderId, Guid couponId)
    {
        var result = new ServiceObjectResult<bool>();
        try
        {
            var token = _tokenAccessor.GetToken();
            if (token == null)
            {
                result.Fail("User not authenticated");
                return result;
            }
            var userId = token.UserId;

            var order = await _context.Orders.FirstOrDefaultAsync(o => o.Id == orderId && o.UserId == userId);
            if (order == null)
            {
                result.Fail("Sipariş bulunamadı");
                return result;
            }

            var coupon = await _context.Coupons.FirstOrDefaultAsync(c => c.Id == couponId && c.DeletedDate == null);
            if (coupon == null)
            {
                result.Fail("Kupon bulunamadı");
                return result;
            }

            // Update order
            order.CouponId = couponId;
            order.CouponCode = coupon.Code;

            // Update coupon usage count
            coupon.CurrentUsageCount++;

            // Update or create user coupon record
            var userCoupon = await _context.UserCoupons
                .FirstOrDefaultAsync(uc => uc.UserId == userId && uc.CouponId == couponId);

            if (userCoupon == null)
            {
                userCoupon = new Domain.Entities.Buyer.UserCoupon
                {
                    Id = Guid.NewGuid(),
                    UserId = userId,
                    CouponId = couponId,
                    OrderId = orderId,
                    UsedAt = DateTime.UtcNow,
                    UsageCount = 1
                };
                await _context.UserCoupons.AddAsync(userCoupon);
            }
            else
            {
                userCoupon.UsageCount++;
                userCoupon.OrderId = orderId;
                userCoupon.UsedAt = DateTime.UtcNow;
            }

            await _context.SaveChangesAsync();
            result.SetData(true);
        }
        catch (Exception e)
        {
            result.Fail(e);
        }

        return result;
    }

    private decimal CalculateDiscount(Coupon coupon, List<CouponCartItemDto>? items, decimal orderAmount)
    {
        if (items == null || items.Count == 0)
        {
            return CalculateDiscountByType(coupon, orderAmount);
        }

        // Filter applicable items
        var applicableItems = GetApplicableItems(coupon, items);
        var applicableTotal = applicableItems.Sum(i => i.UnitPrice * i.Quantity);

        if (applicableTotal == 0 && coupon.ApplicableType != CouponServiceEnums.CouponApplicableTypeEnums.AllItems)
        {
            return 0;
        }

        var amount = coupon.ApplicableType == CouponServiceEnums.CouponApplicableTypeEnums.AllItems ? orderAmount : applicableTotal;
        return CalculateDiscountByType(coupon, amount, applicableItems);
    }

    private List<CouponCartItemDto> GetApplicableItems(Coupon coupon, List<CouponCartItemDto> items)
    {
        if (coupon.ApplicableType == CouponServiceEnums.CouponApplicableTypeEnums.AllItems)
            return items;

        if (coupon.ApplicableType == CouponServiceEnums.CouponApplicableTypeEnums.SpecificMenus)
            return items.Where(i => coupon.CouponMenus?.Any(cm => cm.MenuId == i.MenuId) == true).ToList();

        if (coupon.ApplicableType == CouponServiceEnums.CouponApplicableTypeEnums.SpecificCategories)
        {
            // Get menu-category mapping from CategoryDetail table
            var menuIds = items.Select(i => i.MenuId).ToList();
            var menuCategoryMap = _context.Set<Domain.Entities.Seller.CategoryDetail>()
                .Where(cd => menuIds.Contains(cd.MenuId))
                .Select(cd => new { cd.MenuId, cd.CategoryId })
                .ToList();

            var couponCategoryIds = coupon.CouponCategories?.Select(cc => cc.CategoryId).ToHashSet() ?? new HashSet<Guid>();

            return items.Where(i =>
                menuCategoryMap.Any(mc => mc.MenuId == i.MenuId && couponCategoryIds.Contains(mc.CategoryId))
            ).ToList();
        }

        return items;
    }

    private decimal CalculateDiscountByType(Coupon coupon, decimal amount, List<CouponCartItemDto>? applicableItems = null)
    {
        var discount = coupon.Type switch
        {
            CouponServiceEnums.CouponTypeEnums.Percentage => amount * (coupon.Value / 100m),
            CouponServiceEnums.CouponTypeEnums.FixedAmount => coupon.Value,
            CouponServiceEnums.CouponTypeEnums.BuyXGetY => CalculateBuyXGetYDiscount(coupon, applicableItems),
            _ => 0
        };

        // Apply max discount limit for percentage
        if (coupon.Type == CouponServiceEnums.CouponTypeEnums.Percentage && coupon.MaxDiscountAmount.HasValue)
        {
            discount = Math.Min(discount, coupon.MaxDiscountAmount.Value);
        }

        // Discount can't exceed order amount
        return Math.Min(discount, amount);
    }

    private decimal CalculateBuyXGetYDiscount(Coupon coupon, List<CouponCartItemDto>? items)
    {
        if (items == null || items.Count == 0)
            return 0;

        var totalDiscount = 0m;
        var totalQuantity = items.Sum(i => i.Quantity);

        // How many free items
        var freeSets = totalQuantity / (coupon.BuyQuantity + coupon.GetQuantity);
        var freeItems = freeSets * coupon.GetQuantity;

        if (freeItems == 0)
            return 0;

        // Sort items by price ascending to give cheapest items free
        var sortedItems = items.OrderBy(i => i.UnitPrice).ToList();

        var remainingFree = freeItems;
        foreach (var item in sortedItems)
        {
            if (remainingFree <= 0) break;

            var freeFromThisItem = Math.Min(remainingFree, item.Quantity);
            totalDiscount += freeFromThisItem * item.UnitPrice;
            remainingFree -= freeFromThisItem;
        }

        return totalDiscount;
    }

    private decimal CalculateDiscountPreview(Coupon coupon, decimal orderAmount)
    {
        return coupon.Type switch
        {
            CouponServiceEnums.CouponTypeEnums.Percentage => Math.Min(orderAmount * (coupon.Value / 100m), coupon.MaxDiscountAmount ?? decimal.MaxValue),
            CouponServiceEnums.CouponTypeEnums.FixedAmount => Math.Min(coupon.Value, orderAmount),
            CouponServiceEnums.CouponTypeEnums.BuyXGetY => 0, // Can't preview without items
            _ => 0
        };
    }

    private string GetDiscountDescription(Coupon coupon, decimal discountAmount)
    {
        return coupon.Type switch
        {
            CouponServiceEnums.CouponTypeEnums.Percentage => $"%{coupon.Value} indirim (₺{discountAmount:F2})",
            CouponServiceEnums.CouponTypeEnums.FixedAmount => $"₺{discountAmount:F2} indirim",
            CouponServiceEnums.CouponTypeEnums.BuyXGetY => $"{coupon.BuyQuantity} al {coupon.GetQuantity} öde (₺{discountAmount:F2} tasarruf)",
            _ => ""
        };
    }
}
