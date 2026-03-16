using Application.Services.Courier;
using Domain.Dto.Courier;
using Domain.Service;
using Microsoft.AspNetCore.Mvc;
using WebAPI.Helpers;

namespace WebAPI.Controllers.Courier;

[Route("v1/courier/auth")]
[ApiController]
public class CourierAuthController : BaseController
{
    private readonly ICourierAuthService _courierAuthService;

    public CourierAuthController(ICourierAuthService courierAuthService) => _courierAuthService = courierAuthService;

    [HttpPost("register")]
    public async Task<ServiceObjectResult<bool>> Register([FromBody] CourierRegisterRequestDto request)
        => await _courierAuthService.RegisterAsync(request);

    [HttpPost("login")]
    public async Task<ServiceObjectResult<CourierLoginResponseDto>> Login([FromBody] CourierLoginRequestDto request)
        => await _courierAuthService.LoginAsync(request);
}
