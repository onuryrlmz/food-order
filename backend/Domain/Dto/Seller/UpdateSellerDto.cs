namespace Domain.Dto.Seller;

public class UpdateSellerDto
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string LegalName { get; set; }
    public string? TaxCode { get; set; }
    public string? TaxArea { get; set; }
    public string? IBAN { get; set; }
    public bool IsEInvoiceAvaible { get; set; }
}