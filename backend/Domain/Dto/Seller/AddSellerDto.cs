namespace Domain.Dto.Seller;

public class AddSellerDto
{
    //Satıcı firma bilgileri
    public short CompanyType { get; set; }
    public string Name { get; set; }
    public string LegalName { get; set; }
    public string TaxCode { get; set; }
    public string TaxArea { get; set; }
    public string IBAN { get; set; }
    public bool IsEInvoiceAvaible { get; set; }
    public string? IdentityNumber { get; set; }

    //Satıcı firma sahibi bilgileri
    public string OwnerFirstName { get; set; }
    public string OwnerLastName { get; set; }
    public string OwnerEmail { get; set; }
    public string OwnerPhone { get; set; }
    public string Password { get; set; }

    //Satıcı firma fatura adres bilgileri
    public Guid CityId { get; set; }
    public Guid TownId { get; set; }
    public Guid NeighbourhoodId { get; set; }
    public string AddressLine1 { get; set; }
    public string? AddressLine2 { get; set; }
}