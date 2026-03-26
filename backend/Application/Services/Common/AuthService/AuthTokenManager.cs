using System.Security.Cryptography;
using Base.Entities;
using Base.Enums;
using Base.Security;
using Domain.Dto.Common;
using Domain.Entities.Common;
using Domain.Service;
using Newtonsoft.Json;
using Persistence.IRepositories.Common;
using Persistence.IRepositories.Courier;
using Persistence.IRepositories.Seller;

namespace Application.Services.Common.AuthService;

public class AuthTokenManager : IAuthTokenService
{
    private readonly IRefreshTokenRepository _refreshTokenRepository;
    private readonly IUserRepository _userRepository;
    private readonly IRestaurantRepository _restaurantRepository;
    private readonly ICourierRepository _courierRepository;

    private const int AccessTokenExpirationMinutes = 15;
    private const int RefreshTokenExpirationDays = 30;

    public AuthTokenManager(
        IRefreshTokenRepository refreshTokenRepository,
        IUserRepository userRepository,
        IRestaurantRepository restaurantRepository,
        ICourierRepository courierRepository)
    {
        _refreshTokenRepository = refreshTokenRepository;
        _userRepository = userRepository;
        _restaurantRepository = restaurantRepository;
        _courierRepository = courierRepository;
    }

    public async Task<TokenPairDto> GenerateTokenPairAsync(User user)
    {
        var accessTokenExpiration = DateTime.UtcNow.AddMinutes(AccessTokenExpirationMinutes);

        // Generate access token using existing encrypted JSON pattern
        var tokenDto = new TokenDto
        {
            UserId = user.Id,
            Token = Guid.NewGuid(),
            Expiration = accessTokenExpiration,
            Role = EnumLookups.UserRoleEnumList[user.UserRoleId]
        };

        if (user.UserRoleId is (short)UserRoleEnums.SellerAdmin or (short)UserRoleEnums.SellerUser)
        {
            tokenDto.SellerId = user.SellerId;
            var restaurants = _restaurantRepository.GetList(x => x.SellerId == user.SellerId && !x.DeletedDate.HasValue, size: 100);
            tokenDto.RestaurantIds = restaurants?.Items?.Select(x => x.Id)?.ToList();
        }

        if (user.UserRoleId is (short)UserRoleEnums.Courier or (short)UserRoleEnums.CourierCompanyAdmin)
        {
            var courier = await _courierRepository.GetAsync(x => x.UserId == user.Id && !x.DeletedDate.HasValue);
            if (courier != null)
            {
                tokenDto.CourierId = courier.Id;
                tokenDto.CourierCompanyId = courier.CourierCompanyId;
            }
        }

        var tokenStringData = JsonConvert.SerializeObject(tokenDto);
        var accessToken = tokenStringData.Encrypt();

        // Generate refresh token
        var refreshTokenString = GenerateSecureToken();
        var refreshToken = new RefreshToken
        {
            Id = Guid.NewGuid(),
            UserId = user.Id,
            Token = refreshTokenString,
            ExpiresAt = DateTime.UtcNow.AddDays(RefreshTokenExpirationDays)
        };

        await _refreshTokenRepository.AddAsync(refreshToken);

        return new TokenPairDto
        {
            AccessToken = accessToken,
            RefreshToken = refreshTokenString,
            AccessTokenExpiration = accessTokenExpiration
        };
    }

    public async Task<ServiceObjectResult<TokenPairDto>> RefreshTokenAsync(string refreshToken)
    {
        var response = new ServiceObjectResult<TokenPairDto>();
        try
        {
            var existingToken = await _refreshTokenRepository.GetAsync(x => x.Token == refreshToken && !x.RevokedAt.HasValue && x.ExpiresAt > DateTime.UtcNow);

            if (existingToken == null)
            {
                response.Fail("Geçersiz veya süresi dolmuş refresh token.");
                return response;
            }

            // Revoke old token (rotation)
            existingToken.RevokedAt = DateTime.UtcNow;

            var user = await _userRepository.GetAsync(x => x.Id == existingToken.UserId &&
                                                           x.UserStatusId == (short)UserStatusEnums.Active);

            if (user == null)
            {
                response.Fail("Kullanıcı bulunamadı veya aktif değil.");
                return response;
            }

            // Generate new token pair
            var tokenPair = await GenerateTokenPairAsync(user);

            // Link old token to new one
            existingToken.ReplacedByToken = tokenPair.RefreshToken;
            await _refreshTokenRepository.UpdateAsync(existingToken);

            response.SetData(tokenPair);
        }
        catch (Exception e)
        {
            response.Fail(e);
        }

        return response;
    }

    public async Task RevokeRefreshTokenAsync(string refreshToken)
    {
        var existingToken = await _refreshTokenRepository.GetAsync(x => x.Token == refreshToken && !x.RevokedAt.HasValue);

        if (existingToken != null)
        {
            existingToken.RevokedAt = DateTime.UtcNow;
            await _refreshTokenRepository.UpdateAsync(existingToken);
        }
    }

    public async Task RevokeAllUserTokensAsync(Guid userId)
    {
        var tokens = await _refreshTokenRepository.GetListAsync(x => x.UserId == userId && !x.RevokedAt.HasValue);

        foreach (var token in tokens.Items)
        {
            token.RevokedAt = DateTime.UtcNow;
            await _refreshTokenRepository.UpdateAsync(token);
        }
    }

    private static string GenerateSecureToken()
    {
        var randomBytes = new byte[64];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomBytes);
        return Convert.ToBase64String(randomBytes);
    }
}