using Base.Entities;

namespace Domain.Dto.Admin.Courier;

public class AdminCourierListDto : IDto
{
    public Guid UserId { get; set; }
    public string Email { get; set; }
    public string PhoneNumber { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public short UserStatusId { get; set; }
    public string UserStatusName { get; set; }
    public DateTime CreatedDate { get; set; }
}
