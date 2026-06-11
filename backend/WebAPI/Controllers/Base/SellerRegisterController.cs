using Application.Services.Seller.SellerService;
using Domain.Dto.Seller;
using Domain.Service;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using WebAPI.Helpers;

namespace WebAPI.Controllers.Base;

[Route("v1/seller")]
[ApiController]
public class SellerRegisterController : BaseController
{
    private readonly ISellerService _sellerService;

    public SellerRegisterController(ISellerService sellerService)
    {
        _sellerService = sellerService;
    }

    [HttpPost("register")]
    [EnableRateLimiting("auth")]
    public async Task<ServiceObjectResult<bool>> Register([FromBody] AddSellerDto requestDto)
    {
        return await _sellerService.AddSeller(requestDto);
    }
}
