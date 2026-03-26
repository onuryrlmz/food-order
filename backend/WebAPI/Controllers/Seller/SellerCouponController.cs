using Application.Services.Seller.CouponService;
using Base.Enums;
using Domain.Dto.Seller.Coupon;
using Domain.Service;
using Microsoft.AspNetCore.Mvc;
using WebAPI.Helpers;

namespace WebAPI.Controllers.Seller;

[Route("v1/seller/coupon")]
[ApiController]
public class SellerCouponController : BaseController
{
    private readonly ICouponService _couponService;

    public SellerCouponController(ICouponService couponService)
    {
        _couponService = couponService;
    }

    [HttpGet("list")]
    [AuthorizeAPIRequest(true, false,
        UserRoleEnums.SellerAdmin,
        UserRoleEnums.SellerUser)]
    public async Task<ServiceCollectionResult<GetCouponListDto>> GetList()
    {
        return await _couponService.GetCouponsBySeller();
    }

    [HttpGet("{id}")]
    [AuthorizeAPIRequest(true, false,
        UserRoleEnums.SellerAdmin,
        UserRoleEnums.SellerUser)]
    public async Task<ServiceObjectResult<GetCouponDetailDto>> GetById(Guid id)
    {
        return await _couponService.GetCouponById(id);
    }

    [HttpPost("create")]
    [AuthorizeAPIRequest(true, false, UserRoleEnums.SellerAdmin)]
    public async Task<ServiceObjectResult<Guid>> Create([FromBody] CreateCouponDto requestDto)
    {
        return await _couponService.CreateCoupon(requestDto);
    }

    [HttpPut("update")]
    [AuthorizeAPIRequest(true, false, UserRoleEnums.SellerAdmin)]
    public async Task<ServiceObjectResult<bool>> Update([FromBody] UpdateCouponDto requestDto)
    {
        return await _couponService.UpdateCoupon(requestDto);
    }

    [HttpDelete("{id}")]
    [AuthorizeAPIRequest(true, false, UserRoleEnums.SellerAdmin)]
    public async Task<ServiceObjectResult<bool>> Delete(Guid id)
    {
        return await _couponService.DeleteCoupon(id);
    }
}