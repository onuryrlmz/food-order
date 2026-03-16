using Base.Entities;

namespace Domain.Dto.Courier.Company;

public class CompanyInviteDto : IDto
{
    public Guid Id { get; set; }
    public Guid CourierCompanyId { get; set; }
    public string CompanyName { get; set; }
    public string ContactEmail { get; set; }
    public DateTime RequestedAt { get; set; }
}
