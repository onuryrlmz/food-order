using Application.Services.Common.AuthService;
using Application.Services.Common.TokenService;
using AutoMapper;
using Base.Entities;
using Base.Enums;
using Base.Security;
using Domain.Dto.Common;
using Domain.Entities.Common;
using Domain.Service;
using Newtonsoft.Json;
using Persistence.IRepositories.Common;
using Persistence.IRepositories.Seller;

namespace Application.Services.Common.UserService;

public class UserManager : IUserService
{
    private readonly IMapper _mapper;
    private readonly IUserRepository _userRepository;
    private readonly IRestaurantRepository _restaurantRepository;
    private readonly ITokenAccessor _tokenAccessor;
    private readonly IAuthTokenService _authTokenService;

    public UserManager(IUserRepository userRepository, IMapper mapper, IRestaurantRepository restaurantRepository, ITokenAccessor tokenAccessor, IAuthTokenService authTokenService)
    {
        _userRepository = userRepository;
        _mapper = mapper;
        _restaurantRepository = restaurantRepository;
        _tokenAccessor = tokenAccessor;
        _authTokenService = authTokenService;
    }

    public override async Task<ServiceObjectResult<bool>> Register(UserRegisterDto requestDto)
    {
        var response = new ServiceObjectResult<bool>();
        try
        {
            var checkEmail = await _userRepository.GetAsync(x => x.Email == requestDto.Email.ToLowerInvariant(), withDeleted: true);
            if (checkEmail != null)
            {
                response.AddErrorMessage("Bu e-posta adresi zaten kullanılıyor.");
                return response;
            }

            var mappedUser = _mapper.Map<User>(requestDto);
            mappedUser.Email = requestDto.Email.ToLowerInvariant();
            mappedUser.UserStatusId = (short)AuthorizationServiceEnums.UserStatusEnums.WaitingForActivation;
            mappedUser.Password = BCrypt.Net.BCrypt.HashPassword(requestDto.Password, 12);
            mappedUser.ActivationKey = Guid.NewGuid();
            mappedUser.UserRoleId = (short)AuthorizationServiceEnums.UserRoleEnums.User;

            await _userRepository.AddAsync(mappedUser);
            response.SetData(true);
        }
        catch (Exception e)
        {
            response.Fail(e);
        }

        return response;
    }

    public override async Task<ServiceObjectResult<TokenPairDto>> Login(UserLoginDto requestDto)
    {
        var response = new ServiceObjectResult<TokenPairDto>();
        try
        {
            var user = await _userRepository.GetAsync(x =>
                x.Email == requestDto.Email.ToLowerInvariant() &&
                x.UserStatusId == (short)AuthorizationServiceEnums.UserStatusEnums.Active);

            if (user == null || !BCrypt.Net.BCrypt.Verify(requestDto.Password, user.Password))
            {
                response.AddErrorMessage("E-posta veya şifre hatalı.");
                return response;
            }

            var tokenPair = await _authTokenService.GenerateTokenPairAsync(user);

            // Keep backward compatibility: set Token on response for cookie-based auth
            response.Token = tokenPair.AccessToken;
            response.SetData(tokenPair);
        }
        catch (Exception e)
        {
            response.Fail(e);
        }

        return response;
    }

    public override async Task<ServiceObjectResult<GetUserProfileDto>> GetProfile()
    {
        var response = new ServiceObjectResult<GetUserProfileDto>();
        try
        {
            var token = _tokenAccessor.GetToken();
            var user = await _userRepository.GetAsync(x => x.Id == token!.UserId);
            if (user == null)
            {
                response.Fail("Kullanıcı bulunamadı.");
                return response;
            }

            response.SetData(new GetUserProfileDto
            {
                Id = user.Id,
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                PhoneNumber = user.PhoneNumber,
                BirthDate = user.BirthDate,
                SexId = user.SexId
            });
        }
        catch (Exception e)
        {
            response.Fail(e);
        }

        return response;
    }

    public override async Task<ServiceObjectResult<bool>> UpdateProfile(UpdateUserProfileDto requestDto)
    {
        var response = new ServiceObjectResult<bool>();
        try
        {
            var token = _tokenAccessor.GetToken();
            var user = await _userRepository.GetAsync(x => x.Id == token!.UserId, enableTracking: true);
            if (user == null)
            {
                response.Fail("Kullanıcı bulunamadı.");
                return response;
            }

            user.FirstName = requestDto.FirstName;
            user.LastName = requestDto.LastName;
            user.PhoneNumber = requestDto.PhoneNumber;
            user.BirthDate = requestDto.BirthDate;
            user.SexId = requestDto.SexId;
            await _userRepository.UpdateAsync(user);
            response.SetData(true);
        }
        catch (Exception e)
        {
            response.Fail(e);
        }

        return response;
    }

    public override async Task<ServiceObjectResult<bool>> ChangePassword(ChangePasswordDto requestDto)
    {
        var response = new ServiceObjectResult<bool>();
        try
        {
            var token = _tokenAccessor.GetToken();
            var user = await _userRepository.GetAsync(x => x.Id == token!.UserId, enableTracking: true);
            if (user == null)
            {
                response.Fail("Kullanıcı bulunamadı.");
                return response;
            }

            if (!BCrypt.Net.BCrypt.Verify(requestDto.CurrentPassword, user.Password))
            {
                response.Fail("Mevcut şifre hatalı.");
                return response;
            }

            user.Password = BCrypt.Net.BCrypt.HashPassword(requestDto.NewPassword, 12);
            await _userRepository.UpdateAsync(user);
            response.SetData(true);
        }
        catch (Exception e)
        {
            response.Fail(e);
        }

        return response;
    }

    public override async Task<ServiceCollectionResult<GetUserListResponseDto>> GetUserList(int page = 1, int pageSize = 20, short? roleId = null)
    {
        var response = new ServiceCollectionResult<GetUserListResponseDto>();
        try
        {
            pageSize = Math.Min(pageSize, 100);
            var users = await _userRepository.GetListAsync(
                roleId.HasValue ? x => x.UserRoleId == roleId.Value && !x.DeletedDate.HasValue : x => !x.DeletedDate.HasValue,
                q => q.OrderByDescending(u => u.CreatedDate),
                index: page - 1,
                size: pageSize);

            var roleNames = new Dictionary<short, string>
            {
                { 1, "Admin" }, { 2, "User" }, { 3, "Anonim" }, { 4, "SellerAdmin" }, { 5, "SellerUser" }
            };
            var statusNames = new Dictionary<short, string>
            {
                { 0, "Aktivasyon Bekliyor" }, { 1, "Aktif" }, { 2, "Pasif" }, { 3, "Silindi" }
            };

            var dtos = users.Items.Select(u => new GetUserListResponseDto
            {
                Id = u.Id,
                Email = u.Email,
                FirstName = u.FirstName,
                LastName = u.LastName,
                PhoneNumber = u.PhoneNumber,
                UserRoleId = u.UserRoleId,
                UserRoleName = roleNames.GetValueOrDefault(u.UserRoleId, "?"),
                UserStatusId = u.UserStatusId,
                UserStatusName = statusNames.GetValueOrDefault(u.UserStatusId, "?"),
                SellerId = u.SellerId,
                CreatedDate = u.CreatedDate
            }).ToList();

            response.SetData(users.Count, dtos);
        }
        catch (Exception e)
        {
            response.Fail(e);
        }

        return response;
    }
}