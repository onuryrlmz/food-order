using AutoMapper;
using Base.Enums;
using Domain.Dto.Admin.Coupon;
using Domain.Dto.Seller.Coupon;
using Domain.Entities.Seller;
using Domain.Service;
using Microsoft.EntityFrameworkCore;
using Persistence.Contexts;
using Persistence.IRepositories.Common;
using Persistence.IRepositories.Seller;
using Application.Services.Common.TokenService;

namespace Application.Services.Seller.CouponService;

public class CouponManager : ICouponService
{
    private readonly BaseDbContext _context;
    private readonly IMapper _mapper;
    private readonly ITokenAccessor _tokenAccessor;
    private readonly ICouponRepository _couponRepository;
    private readonly IRestaurantRepository _restaurantRepository;

    public CouponManager(
        BaseDbContext context,
        IMapper mapper,
        ITokenAccessor tokenAccessor,
        ICouponRepository couponRepository,
        IRestaurantRepository restaurantRepository)
    {
        _context = context;
        _mapper = mapper;
        _tokenAccessor = tokenAccessor;
        _couponRepository = couponRepository;
        _restaurantRepository = restaurantRepository;
    }

    public async Task<ServiceObjectResult<Guid>> CreateCoupon(CreateCouponDto requestDto)
    {
        var result = new ServiceObjectResult<Guid>();
        try
        {
            var token = _tokenAccessor.GetToken();
            if (token?.SellerId == null)
            {
                result.Fail("Seller not found");
                return result;
            }

            var sellerId = token.SellerId.Value;

            // Check if code already exists
            var existingCoupon = await _couponRepository.GetAsync(c => c.Code.ToLower() == requestDto.Code.ToLower());
            if (existingCoupon != null)
            {
                result.Fail("Bu kupon kodu zaten mevcut");
                return result;
            }

            // Validate restaurant belongs to seller
            if (requestDto.RestaurantId.HasValue)
            {
                var restaurant = await _restaurantRepository.GetAsync(r => r.Id == requestDto.RestaurantId.Value && r.SellerId == sellerId);
                if (restaurant == null)
                {
                    result.Fail("Restoran bulunamadı");
                    return result;
                }
            }

            var coupon = _mapper.Map<Coupon>(requestDto);
            coupon.Id = Guid.NewGuid();
            coupon.SellerId = sellerId;
            coupon.CurrentUsageCount = 0;

            await _couponRepository.AddAsync(coupon);

            // Add applicable menus
            if (requestDto.ApplicableType == CouponServiceEnums.CouponApplicableTypeEnums.SpecificMenus && requestDto.ApplicableMenuIds != null)
                foreach (var menuId in requestDto.ApplicableMenuIds)
                    _context.CouponMenus.Add(new CouponMenu { CouponId = coupon.Id, MenuId = menuId });

            // Add applicable categories
            if (requestDto.ApplicableType == CouponServiceEnums.CouponApplicableTypeEnums.SpecificCategories && requestDto.ApplicableCategoryIds != null)
                foreach (var categoryId in requestDto.ApplicableCategoryIds)
                    _context.CouponCategories.Add(new CouponCategory { CouponId = coupon.Id, CategoryId = categoryId });

            await _context.SaveChangesAsync();
            result.SetData(coupon.Id);
        }
        catch (Exception e)
        {
            result.Fail(e);
        }

        return result;
    }

    public async Task<ServiceObjectResult<bool>> UpdateCoupon(UpdateCouponDto requestDto)
    {
        var result = new ServiceObjectResult<bool>();
        try
        {
            var token = _tokenAccessor.GetToken();
            if (token?.SellerId == null)
            {
                result.Fail("Seller not found");
                return result;
            }

            var sellerId = token.SellerId.Value;

            var coupon = await _couponRepository.GetAsync(c => c.Id == requestDto.Id && c.SellerId == sellerId);
            if (coupon == null)
            {
                result.Fail("Kupon bulunamadı");
                return result;
            }

            // Check if new code conflicts with another coupon
            if (coupon.Code.ToLower() != requestDto.Code.ToLower())
            {
                var existingCoupon = await _couponRepository.GetAsync(c => c.Code.ToLower() == requestDto.Code.ToLower() && c.Id != requestDto.Id);
                if (existingCoupon != null)
                {
                    result.Fail("Bu kupon kodu zaten mevcut");
                    return result;
                }
            }

            // Manual mapping to prevent overwriting SellerId/CurrentUsageCount
            coupon.Code = requestDto.Code;
            coupon.Name = requestDto.Name;
            coupon.Description = requestDto.Description;
            coupon.Type = requestDto.Type;
            coupon.Value = requestDto.Value;
            coupon.MaxDiscountAmount = requestDto.MaxDiscountAmount;
            coupon.BuyQuantity = requestDto.BuyQuantity;
            coupon.GetQuantity = requestDto.GetQuantity;
            coupon.MinOrderAmount = requestDto.MinOrderAmount;
            coupon.RestaurantId = requestDto.RestaurantId;
            coupon.ApplicableType = requestDto.ApplicableType;
            coupon.StartDate = requestDto.StartDate;
            coupon.EndDate = requestDto.EndDate;
            coupon.UsageLimit = requestDto.UsageLimit;
            coupon.UsagePerUser = requestDto.UsagePerUser;
            _couponRepository.Update(coupon);

            // Update applicable menus
            _context.CouponMenus.RemoveRange(_context.CouponMenus.Where(cm => cm.CouponId == coupon.Id));
            if (requestDto.ApplicableType == CouponServiceEnums.CouponApplicableTypeEnums.SpecificMenus && requestDto.ApplicableMenuIds != null)
                foreach (var menuId in requestDto.ApplicableMenuIds)
                    _context.CouponMenus.Add(new CouponMenu { CouponId = coupon.Id, MenuId = menuId });

            // Update applicable categories
            _context.CouponCategories.RemoveRange(_context.CouponCategories.Where(cc => cc.CouponId == coupon.Id));
            if (requestDto.ApplicableType == CouponServiceEnums.CouponApplicableTypeEnums.SpecificCategories && requestDto.ApplicableCategoryIds != null)
                foreach (var categoryId in requestDto.ApplicableCategoryIds)
                    _context.CouponCategories.Add(new CouponCategory { CouponId = coupon.Id, CategoryId = categoryId });

            await _context.SaveChangesAsync();
            result.SetData(true);
        }
        catch (Exception e)
        {
            result.Fail(e);
        }

        return result;
    }

    public async Task<ServiceObjectResult<bool>> DeleteCoupon(Guid id)
    {
        var result = new ServiceObjectResult<bool>();
        try
        {
            var token = _tokenAccessor.GetToken();
            if (token?.SellerId == null)
            {
                result.Fail("Seller not found");
                return result;
            }

            var sellerId = token.SellerId.Value;

            var coupon = await _couponRepository.GetAsync(c => c.Id == id && c.SellerId == sellerId);
            if (coupon == null)
            {
                result.Fail("Kupon bulunamadı");
                return result;
            }

            coupon.DeletedDate = DateTime.UtcNow;
            _couponRepository.Update(coupon);
            await _context.SaveChangesAsync();
            result.SetData(true);
        }
        catch (Exception e)
        {
            result.Fail(e);
        }

        return result;
    }

    public async Task<ServiceCollectionResult<GetCouponListDto>> GetCouponsBySeller()
    {
        var result = new ServiceCollectionResult<GetCouponListDto>();
        try
        {
            var token = _tokenAccessor.GetToken();
            if (token?.SellerId == null)
            {
                result.Fail("Seller not found");
                return result;
            }

            var sellerId = token.SellerId.Value;

            var coupons = await _context.Coupons
                .Include(c => c.Restaurant)
                .Include(c => c.CouponMenus).ThenInclude(cm => cm.Menu)
                .Include(c => c.CouponCategories).ThenInclude(cc => cc.Category)
                .Where(c => c.SellerId == sellerId && c.DeletedDate == null)
                .OrderByDescending(c => c.CreatedDate)
                .ToListAsync();

            var dtoList = coupons.Select(c => new GetCouponListDto
            {
                Id = c.Id,
                SellerId = c.SellerId,
                RestaurantId = c.RestaurantId,
                RestaurantName = c.Restaurant?.Name,
                Code = c.Code,
                Name = c.Name,
                Description = c.Description,
                Type = c.Type,
                Value = c.Value,
                MaxDiscountAmount = c.MaxDiscountAmount,
                BuyQuantity = c.BuyQuantity,
                GetQuantity = c.GetQuantity,
                MinOrderAmount = c.MinOrderAmount,
                ApplicableType = c.ApplicableType,
                StartDate = c.StartDate,
                EndDate = c.EndDate,
                UsageLimit = c.UsageLimit,
                UsagePerUser = c.UsagePerUser,
                CurrentUsageCount = c.CurrentUsageCount,
                CreatedDate = c.CreatedDate,
                ApplicableMenus = c.CouponMenus?.Select(cm => new CouponApplicableItemDto { Id = cm.MenuId, Name = cm.Menu?.Name ?? "" }).ToList() ?? new List<CouponApplicableItemDto>(),
                ApplicableCategories = c.CouponCategories?.Select(cc => new CouponApplicableItemDto { Id = cc.CategoryId, Name = cc.Category?.Name ?? "" }).ToList() ?? new List<CouponApplicableItemDto>()
            }).ToList();

            result.SetData(dtoList);
        }
        catch (Exception e)
        {
            result.Fail(e);
        }

        return result;
    }

    public async Task<ServiceObjectResult<GetCouponDetailDto>> GetCouponById(Guid id)
    {
        var result = new ServiceObjectResult<GetCouponDetailDto>();
        try
        {
            var token = _tokenAccessor.GetToken();
            if (token?.SellerId == null)
            {
                result.Fail("Seller not found");
                return result;
            }

            var sellerId = token.SellerId.Value;

            var coupon = await _context.Coupons
                .Include(c => c.Restaurant)
                .Include(c => c.CouponMenus).ThenInclude(cm => cm.Menu)
                .Include(c => c.CouponCategories).ThenInclude(cc => cc.Category)
                .FirstOrDefaultAsync(c => c.Id == id && c.SellerId == sellerId && c.DeletedDate == null);

            if (coupon == null)
            {
                result.Fail("Kupon bulunamadı");
                return result;
            }

            var dto = new GetCouponDetailDto
            {
                Id = coupon.Id,
                RestaurantId = coupon.RestaurantId,
                RestaurantName = coupon.Restaurant?.Name,
                Code = coupon.Code,
                Name = coupon.Name,
                Description = coupon.Description,
                Type = coupon.Type,
                Value = coupon.Value,
                MaxDiscountAmount = coupon.MaxDiscountAmount,
                BuyQuantity = coupon.BuyQuantity,
                GetQuantity = coupon.GetQuantity,
                MinOrderAmount = coupon.MinOrderAmount,
                ApplicableType = coupon.ApplicableType,
                StartDate = coupon.StartDate,
                EndDate = coupon.EndDate,
                UsageLimit = coupon.UsageLimit,
                UsagePerUser = coupon.UsagePerUser,
                CurrentUsageCount = coupon.CurrentUsageCount,
                CreatedDate = coupon.CreatedDate,
                ApplicableMenus = coupon.CouponMenus?.Select(cm => new CouponApplicableItemDto
                {
                    Id = cm.MenuId,
                    Name = cm.Menu?.Name ?? ""
                }).ToList() ?? new List<CouponApplicableItemDto>(),
                ApplicableCategories = coupon.CouponCategories?.Select(cc => new CouponApplicableItemDto
                {
                    Id = cc.CategoryId,
                    Name = cc.Category?.Name ?? ""
                }).ToList() ?? new List<CouponApplicableItemDto>()
            };

            result.SetData(dto);
        }
        catch (Exception e)
        {
            result.Fail(e);
        }

        return result;
    }

    public async Task<ServiceCollectionResult<GetCouponListDto>> GetAllCoupons(int page = 1, int pageSize = 50)
    {
        var result = new ServiceCollectionResult<GetCouponListDto>();
        try
        {
            var coupons = await _context.Coupons
                .Include(c => c.Restaurant)
                .Include(c => c.Seller)
                .Include(c => c.CouponMenus).ThenInclude(cm => cm.Menu)
                .Include(c => c.CouponCategories).ThenInclude(cc => cc.Category)
                .Where(c => c.DeletedDate == null)
                .OrderByDescending(c => c.CreatedDate)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var dtoList = coupons.Select(c => new GetCouponListDto
            {
                Id = c.Id,
                SellerId = c.SellerId,
                SellerName = c.Seller?.Name,
                RestaurantId = c.RestaurantId,
                RestaurantName = c.Restaurant?.Name,
                Code = c.Code,
                Name = c.Name,
                Description = c.Description,
                Type = c.Type,
                Value = c.Value,
                MaxDiscountAmount = c.MaxDiscountAmount,
                BuyQuantity = c.BuyQuantity,
                GetQuantity = c.GetQuantity,
                MinOrderAmount = c.MinOrderAmount,
                ApplicableType = c.ApplicableType,
                StartDate = c.StartDate,
                EndDate = c.EndDate,
                UsageLimit = c.UsageLimit,
                UsagePerUser = c.UsagePerUser,
                CurrentUsageCount = c.CurrentUsageCount,
                CreatedDate = c.CreatedDate,
                ApplicableMenus = c.CouponMenus?.Select(cm => new CouponApplicableItemDto { Id = cm.MenuId, Name = cm.Menu?.Name ?? "" }).ToList() ?? new List<CouponApplicableItemDto>(),
                ApplicableCategories = c.CouponCategories?.Select(cc => new CouponApplicableItemDto { Id = cc.CategoryId, Name = cc.Category?.Name ?? "" }).ToList() ?? new List<CouponApplicableItemDto>()
            }).ToList();

            result.SetData(dtoList);
        }
        catch (Exception e)
        {
            result.Fail(e);
        }

        return result;
    }

    public async Task<ServiceObjectResult<GetCouponDetailDto>> AdminGetCouponById(Guid id)
    {
        var result = new ServiceObjectResult<GetCouponDetailDto>();
        try
        {
            var coupon = await _context.Coupons
                .Include(c => c.Restaurant)
                .Include(c => c.Seller)
                .Include(c => c.CouponMenus).ThenInclude(cm => cm.Menu)
                .Include(c => c.CouponCategories).ThenInclude(cc => cc.Category)
                .FirstOrDefaultAsync(c => c.Id == id && c.DeletedDate == null);

            if (coupon == null)
            {
                result.Fail("Kupon bulunamadı");
                return result;
            }

            var dto = new GetCouponDetailDto
            {
                Id = coupon.Id,
                SellerId = coupon.SellerId,
                SellerName = coupon.Seller?.Name,
                RestaurantId = coupon.RestaurantId,
                RestaurantName = coupon.Restaurant?.Name,
                Code = coupon.Code,
                Name = coupon.Name,
                Description = coupon.Description,
                Type = coupon.Type,
                Value = coupon.Value,
                MaxDiscountAmount = coupon.MaxDiscountAmount,
                BuyQuantity = coupon.BuyQuantity,
                GetQuantity = coupon.GetQuantity,
                MinOrderAmount = coupon.MinOrderAmount,
                ApplicableType = coupon.ApplicableType,
                StartDate = coupon.StartDate,
                EndDate = coupon.EndDate,
                UsageLimit = coupon.UsageLimit,
                UsagePerUser = coupon.UsagePerUser,
                CurrentUsageCount = coupon.CurrentUsageCount,
                CreatedDate = coupon.CreatedDate,
                ApplicableMenus = coupon.CouponMenus?.Select(cm => new CouponApplicableItemDto
                {
                    Id = cm.MenuId,
                    Name = cm.Menu?.Name ?? ""
                }).ToList() ?? new List<CouponApplicableItemDto>(),
                ApplicableCategories = coupon.CouponCategories?.Select(cc => new CouponApplicableItemDto
                {
                    Id = cc.CategoryId,
                    Name = cc.Category?.Name ?? ""
                }).ToList() ?? new List<CouponApplicableItemDto>()
            };

            result.SetData(dto);
        }
        catch (Exception e)
        {
            result.Fail(e);
        }

        return result;
    }

    public async Task<ServiceObjectResult<Guid>> AdminCreateCoupon(AdminCreateCouponDto requestDto)
    {
        var result = new ServiceObjectResult<Guid>();
        try
        {
            // Check if code already exists
            var existingCoupon = await _couponRepository.GetAsync(c => c.Code.ToLower() == requestDto.Code.ToLower());
            if (existingCoupon != null)
            {
                result.Fail("Bu kupon kodu zaten mevcut");
                return result;
            }

            // Validate restaurant belongs to seller
            if (requestDto.RestaurantId.HasValue)
            {
                var restaurant = await _restaurantRepository.GetAsync(r => r.Id == requestDto.RestaurantId.Value && r.SellerId == requestDto.SellerId);
                if (restaurant == null)
                {
                    result.Fail("Restoran bulunamadı veya bu satıcıya ait değil");
                    return result;
                }
            }

            var coupon = _mapper.Map<Coupon>(requestDto);
            coupon.Id = Guid.NewGuid();
            coupon.SellerId = requestDto.SellerId;
            coupon.CurrentUsageCount = 0;

            await _couponRepository.AddAsync(coupon);

            // Add applicable menus
            if (requestDto.ApplicableType == CouponServiceEnums.CouponApplicableTypeEnums.SpecificMenus && requestDto.ApplicableMenuIds != null)
                foreach (var menuId in requestDto.ApplicableMenuIds)
                    _context.CouponMenus.Add(new CouponMenu { CouponId = coupon.Id, MenuId = menuId });

            // Add applicable categories
            if (requestDto.ApplicableType == CouponServiceEnums.CouponApplicableTypeEnums.SpecificCategories && requestDto.ApplicableCategoryIds != null)
                foreach (var categoryId in requestDto.ApplicableCategoryIds)
                    _context.CouponCategories.Add(new CouponCategory { CouponId = coupon.Id, CategoryId = categoryId });

            await _context.SaveChangesAsync();
            result.SetData(coupon.Id);
        }
        catch (Exception e)
        {
            result.Fail(e);
        }

        return result;
    }

    public async Task<ServiceObjectResult<bool>> AdminUpdateCoupon(AdminUpdateCouponDto requestDto)
    {
        var result = new ServiceObjectResult<bool>();
        try
        {
            var coupon = await _couponRepository.GetAsync(c => c.Id == requestDto.Id && c.SellerId == requestDto.SellerId);
            if (coupon == null)
            {
                result.Fail("Kupon bulunamadı");
                return result;
            }

            // Check if new code conflicts with another coupon
            if (coupon.Code.ToLower() != requestDto.Code.ToLower())
            {
                var existingCoupon = await _couponRepository.GetAsync(c => c.Code.ToLower() == requestDto.Code.ToLower());
                if (existingCoupon != null)
                {
                    result.Fail("Bu kupon kodu zaten mevcut");
                    return result;
                }
            }

            // Validate restaurant belongs to seller
            if (requestDto.RestaurantId.HasValue)
            {
                var restaurant = await _restaurantRepository.GetAsync(r => r.Id == requestDto.RestaurantId.Value && r.SellerId == requestDto.SellerId);
                if (restaurant == null)
                {
                    result.Fail("Restoran bulunamadı veya bu satıcıya ait değil");
                    return result;
                }
            }

            // Update basic fields
            coupon.Code = requestDto.Code;
            coupon.Name = requestDto.Name;
            coupon.Description = requestDto.Description;
            coupon.Type = requestDto.Type;
            coupon.Value = requestDto.Value;
            coupon.MaxDiscountAmount = requestDto.MaxDiscountAmount;
            coupon.BuyQuantity = requestDto.BuyQuantity;
            coupon.GetQuantity = requestDto.GetQuantity;
            coupon.MinOrderAmount = requestDto.MinOrderAmount;
            coupon.RestaurantId = requestDto.RestaurantId;
            coupon.ApplicableType = requestDto.ApplicableType;
            coupon.StartDate = requestDto.StartDate;
            coupon.EndDate = requestDto.EndDate;
            coupon.UsageLimit = requestDto.UsageLimit;
            coupon.UsagePerUser = requestDto.UsagePerUser;

            // Remove old applicable items
            var oldMenus = await _context.CouponMenus.Where(cm => cm.CouponId == coupon.Id).ToListAsync();
            var oldCategories = await _context.CouponCategories.Where(cc => cc.CouponId == coupon.Id).ToListAsync();
            _context.CouponMenus.RemoveRange(oldMenus);
            _context.CouponCategories.RemoveRange(oldCategories);

            // Add new applicable menus
            if (requestDto.ApplicableType == CouponServiceEnums.CouponApplicableTypeEnums.SpecificMenus && requestDto.ApplicableMenuIds != null)
                foreach (var menuId in requestDto.ApplicableMenuIds)
                    _context.CouponMenus.Add(new CouponMenu { CouponId = coupon.Id, MenuId = menuId });

            // Add new applicable categories
            if (requestDto.ApplicableType == CouponServiceEnums.CouponApplicableTypeEnums.SpecificCategories && requestDto.ApplicableCategoryIds != null)
                foreach (var categoryId in requestDto.ApplicableCategoryIds)
                    _context.CouponCategories.Add(new CouponCategory { CouponId = coupon.Id, CategoryId = categoryId });

            _couponRepository.Update(coupon);
            await _context.SaveChangesAsync();
            result.SetData(true);
        }
        catch (Exception e)
        {
            result.Fail(e);
        }

        return result;
    }

    public async Task<ServiceObjectResult<bool>> AdminDeleteCoupon(Guid id)
    {
        var result = new ServiceObjectResult<bool>();
        try
        {
            var coupon = await _couponRepository.GetAsync(c => c.Id == id);
            if (coupon == null)
            {
                result.Fail("Kupon bulunamadı");
                return result;
            }

            coupon.DeletedDate = DateTime.UtcNow;
            _couponRepository.Update(coupon);
            await _context.SaveChangesAsync();
            result.SetData(true);
        }
        catch (Exception e)
        {
            result.Fail(e);
        }

        return result;
    }
}