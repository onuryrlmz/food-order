namespace Domain.Dto.Seller;

public class GetSellerListResponseDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string LegalName { get; set; } = string.Empty;
    public string TaxCode { get; set; } = string.Empty;
    public short CompanyStatus { get; set; }
    public string CompanyStatusName { get; set; } = string.Empty;
    public short CompanyType { get; set; }
    public string CompanyTypeName { get; set; } = string.Empty;
    public string? OwnerEmail { get; set; }
    public string? OwnerFirstName { get; set; }
    public string? OwnerLastName { get; set; }
    public DateTime CreatedDate { get; set; }
}
