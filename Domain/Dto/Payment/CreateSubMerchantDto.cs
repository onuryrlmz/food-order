namespace Domain.Dto.Payment;

public class CreateSubMerchantDto
{
    public Guid Id { get; set; }
    public short CompanyType { get; set; }
    public string CompanyName { get; set; }
    public string TaxCode { get; set; }
    public string TaxArea { get; set; }
    public string IBAN { get; set; }
    public string Address { get; set; }
    public string Email { get; set; }
}