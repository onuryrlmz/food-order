using Base.Entities;

namespace Domain.Dto.Common;

public class LogoutRequestDto : IDto
{
    public string RefreshToken { get; set; }
}