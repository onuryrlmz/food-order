using Base.Entities;

namespace Domain.Dto.Common;

public class UserLoginDto : IDto
{
    public string Email { get; set; }
    public string Password { get; set; }
}