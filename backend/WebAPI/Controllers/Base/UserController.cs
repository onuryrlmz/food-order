using Application.Services.Common.AuthService;
using Application.Services.Common.PasswordResetService;
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
    private readonly IAuthTokenService _authTokenService;
    private readonly IPasswordResetService _passwordResetService;

    public UserController(IUserService userService, IAuthTokenService authTokenService, IPasswordResetService passwordResetService)
    {
        _userService = userService;
        _authTokenService = authTokenService;
        _passwordResetService = passwordResetService;
    }

    private CookieOptions AuthCookieOptions => new()
    {
        HttpOnly = true,
        Secure = true,
        SameSite = SameSiteMode.None,
        Expires = DateTimeOffset.UtcNow.AddDays(30)
    };

    [HttpPost("register")]
    [EnableRateLimiting("auth")]
    public async Task<ServiceObjectResult<bool>> Register([FromBody] UserRegisterDto requestDto)
    {
        return await _userService.Register(requestDto);
    }

    [HttpPost("login")]
    [EnableRateLimiting("auth")]
    public async Task<ServiceObjectResult<TokenPairDto>> Login([FromBody] UserLoginDto requestDto)
    {
        var result = await _userService.Login(requestDto);
        if (!result.HasFailed && result.Data != null)
            Response.Cookies.Append("auth_token", result.Data.AccessToken, AuthCookieOptions);
        return result;
    }

    [HttpPost("refresh")]
    [EnableRateLimiting("auth")]
    public async Task<ServiceObjectResult<TokenPairDto>> Refresh([FromBody] RefreshRequestDto request)
    {
        var result = await _authTokenService.RefreshTokenAsync(request.RefreshToken);
        if (!result.HasFailed && result.Data != null)
            Response.Cookies.Append("auth_token", result.Data.AccessToken, AuthCookieOptions);
        return result;
    }

    [HttpPost("logout")]
    [AuthorizeAPIRequest(true, false,
        UserRoleEnums.User,
        UserRoleEnums.SellerAdmin,
        UserRoleEnums.SellerUser,
        UserRoleEnums.Admin)]
    public async Task<ServiceObjectResult<bool>> Logout([FromBody] LogoutRequestDto request)
    {
        var response = new ServiceObjectResult<bool>();
        try
        {
            await _authTokenService.RevokeRefreshTokenAsync(request.RefreshToken);
            var opts = AuthCookieOptions;
            opts.Expires = DateTimeOffset.UtcNow.AddDays(-1);
            Response.Cookies.Append("auth_token", "", opts);
            response.SetData(true);
        }
        catch (Exception e)
        {
            response.Fail(e);
        }

        return response;
    }

    [HttpGet("profile")]
    [AuthorizeAPIRequest(true, false,
        UserRoleEnums.User,
        UserRoleEnums.SellerAdmin,
        UserRoleEnums.SellerUser,
        UserRoleEnums.Admin)]
    public async Task<ServiceObjectResult<GetUserProfileDto>> GetProfile()
    {
        return await _userService.GetProfile();
    }

    [HttpPut("profile")]
    [AuthorizeAPIRequest(true, false,
        UserRoleEnums.User,
        UserRoleEnums.SellerAdmin,
        UserRoleEnums.SellerUser,
        UserRoleEnums.Admin)]
    public async Task<ServiceObjectResult<bool>> UpdateProfile([FromBody] UpdateUserProfileDto requestDto)
    {
        return await _userService.UpdateProfile(requestDto);
    }

    [HttpPut("change-password")]
    [AuthorizeAPIRequest(true, false,
        UserRoleEnums.User,
        UserRoleEnums.SellerAdmin,
        UserRoleEnums.SellerUser,
        UserRoleEnums.Admin)]
    public async Task<ServiceObjectResult<bool>> ChangePassword([FromBody] ChangePasswordDto requestDto)
    {
        return await _userService.ChangePassword(requestDto);
    }

    [HttpPost("forgot-password")]
    [EnableRateLimiting("password-reset")]
    public async Task<ServiceObjectResult<bool>> ForgotPassword([FromBody] ForgotPasswordRequestDto request)
    {
        return await _passwordResetService.SendResetCodeAsync(request.EmailOrPhone);
    }

    [HttpPost("verify-reset-code")]
    [EnableRateLimiting("password-reset")]
    public async Task<ServiceObjectResult<bool>> VerifyCode([FromBody] VerifyResetCodeRequestDto request)
    {
        return await _passwordResetService.VerifyCodeAsync(request.EmailOrPhone, request.Code);
    }

    [HttpPost("reset-password")]
    [EnableRateLimiting("password-reset")]
    public async Task<ServiceObjectResult<bool>> ResetPassword([FromBody] ResetPasswordRequestDto request)
    {
        return await _passwordResetService.ResetPasswordAsync(request.EmailOrPhone, request.Code, request.NewPassword);
    }
}