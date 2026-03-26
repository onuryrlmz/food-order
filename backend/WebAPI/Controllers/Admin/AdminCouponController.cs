using Application.Services.Seller.CouponService;
using Base.Enums;
using Domain.Dto.Admin.Coupon;
using Domain.Dto.Seller.Coupon;
using Domain.Service;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Persistence.Contexts;
using WebAPI.Helpers;

namespace WebAPI.Controllers.Admin;

[Route("v1/admin/coupon")]
[ApiController]
public class AdminCouponController : BaseController
{
    private readonly ICouponService _couponService;
    private readonly BaseDbContext _context;

    public AdminCouponController(ICouponService couponService, BaseDbContext context)
    {
        _couponService = couponService;
        _context = context;
    }

    [HttpGet("list")]
    [AuthorizeAPIRequest(true, false, UserRoleEnums.Admin)]
    public async Task<ServiceCollectionResult<GetCouponListDto>> GetList([FromQuery] int page = 1, [FromQuery] int pageSize = 50)
    {
        return await _couponService.GetAllCoupons(page, pageSize);
    }

    [HttpGet("{id:guid}")]
    [AuthorizeAPIRequest(true, false, UserRoleEnums.Admin)]
    public async Task<ServiceObjectResult<GetCouponDetailDto>> GetById(Guid id)
    {
        return await _couponService.AdminGetCouponById(id);
    }

    [HttpPost("create")]
    [AuthorizeAPIRequest(true, false, UserRoleEnums.Admin)]
    public async Task<ServiceObjectResult<Guid>> Create([FromBody] AdminCreateCouponDto requestDto)
    {
        return await _couponService.AdminCreateCoupon(requestDto);
    }

    [HttpPut("update")]
    [AuthorizeAPIRequest(true, false, UserRoleEnums.Admin)]
    public async Task<ServiceObjectResult<bool>> Update([FromBody] AdminUpdateCouponDto requestDto)
    {
        return await _couponService.AdminUpdateCoupon(requestDto);
    }

    [HttpDelete("{id:guid}")]
    [AuthorizeAPIRequest(true, false, UserRoleEnums.Admin)]
    public async Task<ServiceObjectResult<bool>> Delete(Guid id)
    {
        return await _couponService.AdminDeleteCoupon(id);
    }

    [HttpGet("menus/{restaurantId}")]
    [AuthorizeAPIRequest(true, false, UserRoleEnums.Admin)]
    public async Task<IActionResult> GetMenusByRestaurant(Guid restaurantId)
    {
        var menus = await _context.Set<Domain.Entities.Seller.Menu>()
            .Where(m => m.RestaurantId == restaurantId && m.DeletedDate == null)
            .Select(m => new { m.Id, m.Name })
            .ToListAsync();
        return Ok(new { hasFailed = false, data = menus });
    }

    [HttpGet("categories/{restaurantId}")]
    [AuthorizeAPIRequest(true, false, UserRoleEnums.Admin)]
    public async Task<IActionResult> GetCategoriesByRestaurant(Guid restaurantId)
    {
        var categories = await _context.Set<Domain.Entities.Seller.Category>()
            .Where(c => c.RestaurantId == restaurantId && c.DeletedDate == null)
            .Select(c => new { c.Id, c.Name })
            .ToListAsync();
        return Ok(new { hasFailed = false, data = categories });
    }
}