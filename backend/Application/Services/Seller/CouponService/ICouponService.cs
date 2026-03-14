using Domain.Dto.Admin.Coupon;
using Domain.Dto.Seller.Coupon;
using Domain.Service;

namespace Application.Services.Seller.CouponService;

public interface ICouponService
{
    // Seller endpoints
    Task<ServiceObjectResult<Guid>> CreateCoupon(CreateCouponDto requestDto);
    Task<ServiceObjectResult<bool>> UpdateCoupon(UpdateCouponDto requestDto);
    Task<ServiceObjectResult<bool>> DeleteCoupon(Guid id);
    Task<ServiceCollectionResult<GetCouponListDto>> GetCouponsBySeller();
    Task<ServiceObjectResult<GetCouponDetailDto>> GetCouponById(Guid id);
    
    // Admin endpoints
    Task<ServiceCollectionResult<GetCouponListDto>> GetAllCoupons(int page = 1, int pageSize = 50);
    Task<ServiceObjectResult<GetCouponDetailDto>> AdminGetCouponById(Guid id);
    Task<ServiceObjectResult<Guid>> AdminCreateCoupon(AdminCreateCouponDto requestDto);
    Task<ServiceObjectResult<bool>> AdminUpdateCoupon(AdminUpdateCouponDto requestDto);
    Task<ServiceObjectResult<bool>> AdminDeleteCoupon(Guid id);
}
