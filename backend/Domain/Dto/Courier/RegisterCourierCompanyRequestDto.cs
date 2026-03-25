namespace Domain.Dto.Courier;

public class RegisterCourierCompanyRequestDto
{
    public string Name { get; set; }
    public string LegalName { get; set; }
    public string TaxCode { get; set; }
    public string? TaxArea { get; set; }
    public string IBAN { get; set; }
    public string Phone { get; set; }
    public string? Email { get; set; }
    public string? ContactPerson { get; set; }
    public short CompanyTypeId { get; set; }
    public string? IdentityNumber { get; set; }
}
