using Base.Entities;

namespace Domain.Dto.Admin.Courier;

public class ApproveCourierDto : IDto
{
    public Guid UserId { get; set; }
    public bool Approve { get; set; }
}
