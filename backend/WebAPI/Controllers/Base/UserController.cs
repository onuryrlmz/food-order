using Application.Services.Common.UserService;
using Base.Enums;
using Domain.Dto.Common;
using Domain.Service;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using WebAPI.Helpers;

namespace WebAPI.Controllers.Base;

[Route("v1/auth")]
[ApiController]
public class UserController : BaseController
{
    private readonly IUserService _userService;

    public UserController(IUserService userService)
    {
        _userService = userService;
    }

    private CookieOptions AuthCookieOptions => new CookieOptions
    {
        HttpOnly = true,
        Secure = true,
        SameSite = SameSiteMode.None,
        Expires = DateTimeOffset.UtcNow.AddDays(30)
    };

    [HttpPost("register")]
    [EnableRateLimiting("auth")]
    public async Task<ServiceObjectResult<bool>> Register([FromBody] UserRegisterDto requestDto)
        => await _userService.Register(requestDto);

    [HttpPost("login")]
    [EnableRateLimiting("auth")]
    public async Task<ServiceObjectResult<bool>> Login([FromBody] UserLoginDto requestDto)
    {
        var result = await _userService.Login(requestDto);
        if (!result.HasFailed && !string.IsNullOrEmpty(result.Token))
            Response.Cookies.Append("auth_token", result.Token, AuthCookieOptions);
        return result;
    }

    [HttpPost("logout")]
    public IActionResult Logout()
    {
        var opts = AuthCookieOptions;
        opts.Expires = DateTimeOffset.UtcNow.AddDays(-1);
        Response.Cookies.Append("auth_token", "", opts);
        return Ok();
    }

    [HttpGet("profile")]
    [AuthorizeAPIRequest(true, false,
        AuthorizationServiceEnums.UserRoleEnums.User,
        AuthorizationServiceEnums.UserRoleEnums.SellerAdmin,
        AuthorizationServiceEnums.UserRoleEnums.SellerUser,
        AuthorizationServiceEnums.UserRoleEnums.Admin,
        AuthorizationServiceEnums.UserRoleEnums.Courier)]
    public async Task<ServiceObjectResult<GetUserProfileDto>> GetProfile()
        => await _userService.GetProfile();

    [HttpPut("profile")]
    [AuthorizeAPIRequest(true, false,
        AuthorizationServiceEnums.UserRoleEnums.User,
        AuthorizationServiceEnums.UserRoleEnums.SellerAdmin,
        AuthorizationServiceEnums.UserRoleEnums.SellerUser,
        AuthorizationServiceEnums.UserRoleEnums.Admin,
        AuthorizationServiceEnums.UserRoleEnums.Courier)]
    public async Task<ServiceObjectResult<bool>> UpdateProfile([FromBody] UpdateUserProfileDto requestDto)
        => await _userService.UpdateProfile(requestDto);

    [HttpPut("change-password")]
    [AuthorizeAPIRequest(true, false,
        AuthorizationServiceEnums.UserRoleEnums.User,
        AuthorizationServiceEnums.UserRoleEnums.SellerAdmin,
        AuthorizationServiceEnums.UserRoleEnums.SellerUser,
        AuthorizationServiceEnums.UserRoleEnums.Admin,
        AuthorizationServiceEnums.UserRoleEnums.Courier)]
    public async Task<ServiceObjectResult<bool>> ChangePassword([FromBody] ChangePasswordDto requestDto)
        => await _userService.ChangePassword(requestDto);
}
