using Base.Entities;

namespace Domain.Dto.Seller.Courier;

public class InviteCourierRequestDto : IDto
{
    public string Email { get; set; }
}
