namespace Domain.Dto.Payment;

public class UpdateSellerRequestDto
{
    public Guid Id { get; set; }
    public string SellerPaymentId { get; set; }
    public short CompanyType { get; set; }
    public string CompanyName { get; set; }
    public string TaxCode { get; set; }
    public string TaxArea { get; set; }
    public string IBAN { get; set; }
    public string Address { get; set; }
    public string Email { get; set; }
}