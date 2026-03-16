using Base.Entities;

namespace Domain.Dto.Common;

public class ForgotPasswordRequestDto : IDto
{
    public string EmailOrPhone { get; set; }
}
