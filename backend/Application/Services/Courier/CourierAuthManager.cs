using Base.Enums;
using Base.Entities;
using Base.Security;
using Domain.Dto.Courier;
using Domain.Entities.Common;
using Domain.Service;
using Newtonsoft.Json;
using Persistence.IRepositories.Common;

namespace Application.Services.Courier;

public class CourierAuthManager : ICourierAuthService
{
    private readonly IUserRepository _userRepository;

    public CourierAuthManager(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<ServiceObjectResult<bool>> RegisterAsync(CourierRegisterRequestDto request)
    {
        var result = new ServiceObjectResult<bool>();
        try
        {
            var existingUser = await _userRepository.GetAsync(
                x => x.Email == request.Email.ToLowerInvariant(), withDeleted: true);

            if (existingUser != null)
            {
                result.AddErrorMessage("Bu e-posta adresi zaten kullanılıyor.");
                return result;
            }

            var existingPhone = await _userRepository.GetAsync(
                x => x.PhoneNumber == request.PhoneNumber, withDeleted: true);

            if (existingPhone != null)
            {
                result.AddErrorMessage("Bu telefon numarası zaten kullanılıyor.");
                return result;
            }

            var user = new User
            {
                Email = request.Email.ToLowerInvariant(),
                Password = BCrypt.Net.BCrypt.HashPassword(request.Password, workFactor: 12),
                PhoneNumber = request.PhoneNumber,
                FirstName = request.FirstName,
                LastName = request.LastName,
                UserRoleId = (short)AuthorizationServiceEnums.UserRoleEnums.Courier,
                UserStatusId = (short)AuthorizationServiceEnums.UserStatusEnums.WaitingForActivation,
                ActivationKey = Guid.NewGuid()
            };

            await _userRepository.AddAsync(user);
            result.SetData(true);
            result.AddSuccessMessage("Kayıt başarılı. Hesabınız admin onayından sonra aktif olacaktır.");
        }
        catch (Exception e)
        {
            result.Fail(e);
        }

        return result;
    }

    public async Task<ServiceObjectResult<CourierLoginResponseDto>> LoginAsync(CourierLoginRequestDto request)
    {
        var result = new ServiceObjectResult<CourierLoginResponseDto>();
        try
        {
            var user = await _userRepository.GetAsync(x =>
                x.Email == request.Email.ToLowerInvariant() &&
                x.UserRoleId == (short)AuthorizationServiceEnums.UserRoleEnums.Courier);

            if (user == null || !BCrypt.Net.BCrypt.Verify(request.Password, user.Password))
            {
                result.AddErrorMessage("E-posta veya şifre hatalı.");
                return result;
            }

            if (user.UserStatusId == (short)AuthorizationServiceEnums.UserStatusEnums.WaitingForActivation)
            {
                result.AddErrorMessage("Hesabınız henüz onaylanmamış. Lütfen admin onayını bekleyin.");
                return result;
            }

            if (user.UserStatusId != (short)AuthorizationServiceEnums.UserStatusEnums.Active)
            {
                result.AddErrorMessage("Hesabınız aktif değil.");
                return result;
            }

            var token = Guid.NewGuid();
            var expiration = DateTime.UtcNow.AddDays(30);

            var tokenDto = new TokenDto
            {
                UserId = user.Id,
                Token = token,
                Expiration = expiration,
                Role = AuthorizationServiceEnums.UserRoleEnums.Courier
            };

            var tokenStringData = JsonConvert.SerializeObject(tokenDto);
            var encryptedToken = tokenStringData.Encrypt();

            var response = new CourierLoginResponseDto
            {
                UserId = user.Id,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Token = encryptedToken,
                Expiration = expiration
            };

            result.SetData(response);
        }
        catch (Exception e)
        {
            result.Fail(e);
        }

        return result;
    }
}
