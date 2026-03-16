using NArchitecture.Core.Persistence.Repositories;

namespace Domain.Entities.Seller;

public class Seller : Entity<Guid>
{
    public short CompanyType { get; set; }
    public short CompanyStatus { get; set; }
    public string Name { get; set; }
    public string LegalName { get; set; }
    public string TaxCode { get; set; }
    public string TaxArea { get; set; }
    public string IBAN { get; set; }
    public string? ApiKey { get; set; }
    public string? ApiSecret { get; set; }
    public bool IsEInvoiceAvaible { get; set; }
    public string? IdentityNumber { get; set; }
}