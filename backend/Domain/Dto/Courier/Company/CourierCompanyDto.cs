using Base.Entities;

namespace Domain.Dto.Courier.Company;

public class CourierCompanyDto : IDto
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string? LegalName { get; set; }
    public string ContactEmail { get; set; }
    public string? ContactPhone { get; set; }
    public short StatusId { get; set; }
    public string StatusName { get; set; }
    public int MemberCount { get; set; }
    public DateTime CreatedDate { get; set; }
}
