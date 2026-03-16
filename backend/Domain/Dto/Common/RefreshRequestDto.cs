using Base.Entities;

namespace Domain.Dto.Common;

public class RefreshRequestDto : IDto
{
    public string RefreshToken { get; set; }
}
