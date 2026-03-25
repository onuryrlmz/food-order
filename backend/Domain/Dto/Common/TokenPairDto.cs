using Base.Entities;

namespace Domain.Dto.Common;

public class TokenPairDto : IDto
{
    public string AccessToken { get; set; }
    public string RefreshToken { get; set; }
    public DateTime AccessTokenExpiration { get; set; }
}