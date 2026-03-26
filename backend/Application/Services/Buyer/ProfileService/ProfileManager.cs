using Application.Services.Common.TokenService;
using Domain.Dto.Buyer.Profile;
using Domain.Service;
using Microsoft.AspNetCore.Http;
using Persistence.IRepositories;
using Persistence.IRepositories.Common;

namespace Application.Services.Buyer.ProfileService;

public class ProfileManager : IProfileService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ITokenAccessor _tokenAccessor;
    private readonly IUserRepository _userRepository;

    public ProfileManager(IUnitOfWork unitOfWork, ITokenAccessor tokenAccessor, IUserRepository userRepository)
    {
        _unitOfWork = unitOfWork;
        _tokenAccessor = tokenAccessor;
        _userRepository = userRepository;
    }

    public async Task<ServiceObjectResult<ProfileDto>> GetProfile()
    {
        var result = new ServiceObjectResult<ProfileDto>();
        try
        {
            var token = _tokenAccessor.GetToken();
            if (token == null)
            {
                result.Fail("Unauthorized");
                return result;
            }

            var user = await _userRepository.GetAsync(x => x.Id == token.UserId);
            if (user == null)
            {
                result.Fail("User not found");
                return result;
            }

            result.SetData(new ProfileDto
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                BirthDate = user.BirthDate,
                SexId = user.SexId,
                ProfilePhotoUrl = user.ProfilePhotoUrl,
                CreatedDate = user.CreatedDate
            });
        }
        catch (Exception e)
        {
            result.Fail(e);
        }

        return result;
    }

    public async Task<ServiceObjectResult<ProfileDto>> UpdateProfile(UpdateProfileRequestDto requestDto)
    {
        var result = new ServiceObjectResult<ProfileDto>();
        try
        {
            var token = _tokenAccessor.GetToken();
            if (token == null)
            {
                result.Fail("Unauthorized");
                return result;
            }

            var user = await _userRepository.GetAsync(x => x.Id == token.UserId, enableTracking: true);
            if (user == null)
            {
                result.Fail("User not found");
                return result;
            }

            if (!string.IsNullOrWhiteSpace(requestDto.Email) && requestDto.Email != user.Email)
            {
                var existingUser = await _userRepository.GetAsync(x => x.Email == requestDto.Email && x.Id != user.Id);
                if (existingUser != null)
                {
                    result.Fail("This email is already in use");
                    return result;
                }
                user.Email = requestDto.Email;
            }

            if (!string.IsNullOrWhiteSpace(requestDto.FirstName))
                user.FirstName = requestDto.FirstName;

            if (!string.IsNullOrWhiteSpace(requestDto.LastName))
                user.LastName = requestDto.LastName;

            if (!string.IsNullOrWhiteSpace(requestDto.PhoneNumber))
                user.PhoneNumber = requestDto.PhoneNumber;

            await _userRepository.UpdateAsync(user);
            await _unitOfWork.CompleteAsync();

            result.SetData(new ProfileDto
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                BirthDate = user.BirthDate,
                SexId = user.SexId,
                ProfilePhotoUrl = user.ProfilePhotoUrl,
                CreatedDate = user.CreatedDate
            });
        }
        catch (Exception e)
        {
            result.Fail(e);
        }

        return result;
    }

    public async Task<ServiceObjectResult<bool>> ChangePassword(ChangePasswordRequestDto requestDto)
    {
        var result = new ServiceObjectResult<bool>();
        try
        {
            var token = _tokenAccessor.GetToken();
            if (token == null)
            {
                result.Fail("Unauthorized");
                return result;
            }

            if (requestDto.NewPassword != requestDto.ConfirmNewPassword)
            {
                result.Fail("New password and confirmation do not match");
                return result;
            }

            if (requestDto.NewPassword.Length < 6)
            {
                result.Fail("Password must be at least 6 characters");
                return result;
            }

            var user = await _userRepository.GetAsync(x => x.Id == token.UserId, enableTracking: true);
            if (user == null)
            {
                result.Fail("User not found");
                return result;
            }

            if (!BCrypt.Net.BCrypt.Verify(requestDto.CurrentPassword, user.Password))
            {
                result.Fail("Current password is incorrect");
                return result;
            }

            user.Password = BCrypt.Net.BCrypt.HashPassword(requestDto.NewPassword, 12);
            await _userRepository.UpdateAsync(user);
            await _unitOfWork.CompleteAsync();

            result.SetData(true);
        }
        catch (Exception e)
        {
            result.Fail(e);
        }

        return result;
    }

    public async Task<ServiceObjectResult<string>> UploadProfilePhoto(IFormFile file)
    {
        var result = new ServiceObjectResult<string>();
        try
        {
            var token = _tokenAccessor.GetToken();
            if (token == null)
            {
                result.Fail("Unauthorized");
                return result;
            }

            if (file == null || file.Length == 0)
            {
                result.Fail("No file provided");
                return result;
            }

            var allowedTypes = new[] { "image/jpeg", "image/png", "image/webp" };
            if (!allowedTypes.Contains(file.ContentType.ToLower()))
            {
                result.Fail("Only JPEG, PNG and WebP images are allowed");
                return result;
            }

            if (file.Length > 5 * 1024 * 1024) // 5MB
            {
                result.Fail("File size cannot exceed 5MB");
                return result;
            }

            var user = await _userRepository.GetAsync(x => x.Id == token.UserId, enableTracking: true);
            if (user == null)
            {
                result.Fail("User not found");
                return result;
            }

            // TODO: Integrate with IAwsS3ServiceAdapter for actual upload
            var fileName = $"profile/{token.UserId}/{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
            user.ProfilePhotoUrl = fileName;

            await _userRepository.UpdateAsync(user);
            await _unitOfWork.CompleteAsync();

            result.SetData(fileName);
        }
        catch (Exception e)
        {
            result.Fail(e);
        }

        return result;
    }

    public async Task<ServiceObjectResult<bool>> DeleteAccount()
    {
        var result = new ServiceObjectResult<bool>();
        try
        {
            var token = _tokenAccessor.GetToken();
            if (token == null)
            {
                result.Fail("Unauthorized");
                return result;
            }

            var user = await _userRepository.GetAsync(x => x.Id == token.UserId, enableTracking: true);
            if (user == null)
            {
                result.Fail("User not found");
                return result;
            }

            user.UserStatusId = (short)Base.Enums.UserStatusEnums.Deleted;
            user.DeletedDate = DateTime.UtcNow;
            await _userRepository.UpdateAsync(user);
            await _unitOfWork.CompleteAsync();

            result.SetData(true);
        }
        catch (Exception e)
        {
            result.Fail(e);
        }

        return result;
    }
}
