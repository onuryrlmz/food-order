using Application.Services.Buyer.ProfileService;
using Base.Enums;
using Domain.Dto.Buyer.Profile;
using Domain.Service;
using Microsoft.AspNetCore.Mvc;
using WebAPI.Helpers;

namespace WebAPI.Controllers.Customer;

[Route("v1/customer/profile")]
[ApiController]
public class CustomerProfileController : BaseController
{
    private readonly IProfileService _profileService;

    public CustomerProfileController(IProfileService profileService)
    {
        _profileService = profileService;
    }

    [HttpGet]
    [AuthorizeAPIRequest(true, false, UserRoleEnums.User)]
    public async Task<ServiceObjectResult<ProfileDto>> GetProfile()
    {
        return await _profileService.GetProfile();
    }

    [HttpPut]
    [AuthorizeAPIRequest(true, false, UserRoleEnums.User)]
    public async Task<ServiceObjectResult<ProfileDto>> UpdateProfile([FromBody] UpdateProfileRequestDto requestDto)
    {
        return await _profileService.UpdateProfile(requestDto);
    }

    [HttpPut("password")]
    [AuthorizeAPIRequest(true, false, UserRoleEnums.User)]
    public async Task<ServiceObjectResult<bool>> ChangePassword([FromBody] ChangePasswordRequestDto requestDto)
    {
        return await _profileService.ChangePassword(requestDto);
    }

    [HttpPost("photo")]
    [AuthorizeAPIRequest(true, false, UserRoleEnums.User)]
    [Consumes("multipart/form-data")]
    public async Task<ServiceObjectResult<string>> UploadPhoto(IFormFile file)
    {
        return await _profileService.UploadProfilePhoto(file);
    }

    [HttpDelete]
    [AuthorizeAPIRequest(true, false, UserRoleEnums.User)]
    public async Task<ServiceObjectResult<bool>> DeleteAccount()
    {
        return await _profileService.DeleteAccount();
    }
}
