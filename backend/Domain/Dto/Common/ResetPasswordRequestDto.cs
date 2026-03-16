using Base.Entities;

namespace Domain.Dto.Common;

public class ResetPasswordRequestDto : IDto
{
    public string EmailOrPhone { get; set; }
    public string Code { get; set; }
    public string NewPassword { get; set; }
}
