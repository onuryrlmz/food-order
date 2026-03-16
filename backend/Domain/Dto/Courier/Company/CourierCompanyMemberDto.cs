using Base.Entities;

namespace Domain.Dto.Courier.Company;

public class CourierCompanyMemberDto : IDto
{
    public Guid Id { get; set; }
    public Guid CourierId { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string Email { get; set; }
    public string? PhoneNumber { get; set; }
    public short StatusId { get; set; }
    public string StatusName { get; set; }
    public DateTime RequestedAt { get; set; }
    public DateTime? ApprovedAt { get; set; }
}
