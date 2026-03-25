using NArchitecture.Core.Persistence.Repositories;

namespace Domain.Entities.Courier;

public class CourierCompany : Entity<Guid>
{
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
    public string? LogoUrl { get; set; }

    public virtual ICollection<Courier> Couriers { get; set; }
    public virtual ICollection<RestaurantCourierAgreement> Agreements { get; set; }
}