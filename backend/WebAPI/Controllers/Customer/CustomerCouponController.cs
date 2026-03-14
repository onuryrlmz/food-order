using Application.Services.Buyer.CouponService;
using Domain.Dto.Buyer.Coupon;
using Domain.Service;
using Microsoft.AspNetCore.Mvc;
using WebAPI.Helpers;

namespace WebAPI.Controllers.Customer;

[Route("v1/customer/coupon")]
[ApiController]
public class CustomerCouponController : BaseController
{
    private readonly ICouponValidationService _couponValidationService;

    public CustomerCouponController(ICouponValidationService couponValidationService) 
        => _couponValidationService = couponValidationService;

    [HttpPost("validate")]
    [AuthorizeAPIRequest(true, false)]
    public async Task<ServiceObjectResult<ValidateCouponResultDto>> Validate([FromBody] ValidateCouponDto requestDto)
        => await _couponValidationService.ValidateCoupon(requestDto);

    [HttpGet("available/{restaurantId}")]
    [AuthorizeAPIRequest(true, false)]
    public async Task<ServiceCollectionResult<AvailableCouponDto>> GetAvailable(Guid restaurantId, [FromQuery] decimal orderAmount)
        => await _couponValidationService.GetAvailableCoupons(restaurantId, orderAmount);
}
