using Base.Entities;

namespace Domain.Dto.Courier.Company;

public class RegisterCourierCompanyRequestDto : IDto
{
    public string Name { get; set; }
    public string? LegalName { get; set; }
    public string? TaxCode { get; set; }
    public string? TaxArea { get; set; }
    public string ContactEmail { get; set; }
    public string? ContactPhone { get; set; }
}
