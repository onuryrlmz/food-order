using Base.Entities;

namespace Domain.Dto.Courier;

public class CourierLoginRequestDto : IDto
{
    public string Email { get; set; }
    public string Password { get; set; }
}
