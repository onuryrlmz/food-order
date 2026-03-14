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

    public SellerCouponController(ICouponService couponService) => _couponService = couponService;

    [HttpGet("list")]
    [AuthorizeAPIRequest(true, false,
        AuthorizationServiceEnums.UserRoleEnums.SellerAdmin,
        AuthorizationServiceEnums.UserRoleEnums.SellerUser)]
    public async Task<ServiceCollectionResult<GetCouponListDto>> GetList()
        => await _couponService.GetCouponsBySeller();

    [HttpGet("{id}")]
    [AuthorizeAPIRequest(true, false,
        AuthorizationServiceEnums.UserRoleEnums.SellerAdmin,
        AuthorizationServiceEnums.UserRoleEnums.SellerUser)]
    public async Task<ServiceObjectResult<GetCouponDetailDto>> GetById(Guid id)
        => await _couponService.GetCouponById(id);

    [HttpPost("create")]
    [AuthorizeAPIRequest(true, false, AuthorizationServiceEnums.UserRoleEnums.SellerAdmin)]
    public async Task<ServiceObjectResult<Guid>> Create([FromBody] CreateCouponDto requestDto)
        => await _couponService.CreateCoupon(requestDto);

    [HttpPut("update")]
    [AuthorizeAPIRequest(true, false, AuthorizationServiceEnums.UserRoleEnums.SellerAdmin)]
    public async Task<ServiceObjectResult<bool>> Update([FromBody] UpdateCouponDto requestDto)
        => await _couponService.UpdateCoupon(requestDto);

    [HttpDelete("{id}")]
    [AuthorizeAPIRequest(true, false, AuthorizationServiceEnums.UserRoleEnums.SellerAdmin)]
    public async Task<ServiceObjectResult<bool>> Delete(Guid id)
        => await _couponService.DeleteCoupon(id);
}
