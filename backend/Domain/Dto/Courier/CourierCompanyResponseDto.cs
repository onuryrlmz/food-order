namespace Domain.Dto.Courier;

public class CourierCompanyResponseDto
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string LegalName { get; set; }
    public string Phone { get; set; }
    public string? Email { get; set; }
    public string? ContactPerson { get; set; }
    public short StatusId { get; set; }
    public short CompanyTypeId { get; set; }
    public decimal CommissionRate { get; set; }
    public string? LogoUrl { get; set; }
    public int CourierCount { get; set; }
    public DateTime CreatedDate { get; set; }
}