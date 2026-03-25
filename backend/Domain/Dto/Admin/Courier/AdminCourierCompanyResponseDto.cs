namespace Domain.Dto.Admin.Courier;

public class AdminCourierCompanyResponseDto
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string LegalName { get; set; }
    public string TaxCode { get; set; }
    public string? TaxArea { get; set; }
    public string IBAN { get; set; }
    public string Phone { get; set; }
    public string? Email { get; set; }
    public string? ContactPerson { get; set; }
    public short StatusId { get; set; }
    public short CompanyTypeId { get; set; }
    public string? IdentityNumber { get; set; }
    public decimal CommissionRate { get; set; }
    public int CourierCount { get; set; }
    public int TotalDeliveries { get; set; }
    public DateTime CreatedDate { get; set; }
}