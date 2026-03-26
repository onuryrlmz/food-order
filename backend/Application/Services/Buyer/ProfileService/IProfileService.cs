using Domain.Dto.Buyer.Profile;
using Domain.Service;
using Microsoft.AspNetCore.Http;

namespace Application.Services.Buyer.ProfileService;

public interface IProfileService
{
    Task<ServiceObjectResult<ProfileDto>> GetProfile();
    Task<ServiceObjectResult<ProfileDto>> UpdateProfile(UpdateProfileRequestDto requestDto);
    Task<ServiceObjectResult<bool>> ChangePassword(ChangePasswordRequestDto requestDto);
    Task<ServiceObjectResult<string>> UploadProfilePhoto(IFormFile file);
    Task<ServiceObjectResult<bool>> DeleteAccount();
}
