using Base.Entities;

namespace Domain.Dto.Courier;

public class CourierLoginResponseDto : IDto
{
    public Guid UserId { get; set; }
    public string Email { get; set; }
    public string PhoneNumber { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string Token { get; set; }
    public string? RefreshToken { get; set; }
    public DateTime Expiration { get; set; }
}
