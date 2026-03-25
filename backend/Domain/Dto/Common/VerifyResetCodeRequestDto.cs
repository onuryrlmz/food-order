using Base.Entities;

namespace Domain.Dto.Common;

public class VerifyResetCodeRequestDto : IDto
{
    public string EmailOrPhone { get; set; }
    public string Code { get; set; }
}