using Application.Services.Common.UserService;
using Domain.Dto.Common;
using Domain.Service;
using Microsoft.AspNetCore.Mvc;
using WebAPI.Helpers;

namespace WebAPI.Controllers.Common;

[Route("v1/user")]
[ApiController]
public class UserController : BaseController
{
    private readonly IUserService _userService;

    public UserController(IUserService userService)
    {
        _userService = userService;
    }

    [HttpPost("register")]
    public async Task<ServiceObjectResult<bool>> Register([FromBody] UserRegisterDto requestDto)
    {
        return await _userService.Register(requestDto);
    }

    [HttpPost("login")]
    public async Task<ServiceObjectResult<bool>> Login([FromBody] UserLoginDto requestDto)
    {
        return await _userService.Login(requestDto);
    }
}