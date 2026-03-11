using Application.Services.Common.UserService;
using Base.Enums;
using Domain.Dto.Common;
using Domain.Service;
using Microsoft.AspNetCore.Mvc;
using WebAPI.Helpers;

namespace WebAPI.Controllers.Admin;

[Route("v1/admin/user")]
[ApiController]
public class AdminUserController : BaseController
{
    private readonly IUserService _userService;

    public AdminUserController(IUserService userService) => _userService = userService;

    [HttpGet("list")]
    [AuthorizeAPIRequest(true, false, AuthorizationServiceEnums.UserRoleEnums.Admin)]
    public async Task<ServiceCollectionResult<GetUserListResponseDto>> GetList(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] short? roleId = null)
        => await _userService.GetUserList(page, pageSize, roleId);
}
