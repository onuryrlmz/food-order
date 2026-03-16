using Domain.Dto.Common;
using Domain.Entities.Common;
using Domain.Service;

namespace Application.Services.Common.AuthService;

public interface IAuthTokenService
{
    Task<TokenPairDto> GenerateTokenPairAsync(User user);
    Task<ServiceObjectResult<TokenPairDto>> RefreshTokenAsync(string refreshToken);
    Task RevokeRefreshTokenAsync(string refreshToken);
    Task RevokeAllUserTokensAsync(Guid userId);
}
