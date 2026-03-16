using Domain.Entities.Common;
using NArchitecture.Core.Persistence.Repositories;

namespace Domain.Entities.Courier;

public class CourierCompany : Entity<Guid>
{
    public string Name { get; set; }
    public string? LegalName { get; set; }
    public string? TaxCode { get; set; }
    public string? TaxArea { get; set; }
    public string ContactEmail { get; set; }
    public string? ContactPhone { get; set; }
    public Guid OwnerUserId { get; set; }
    public short StatusId { get; set; } = (short)Base.Enums.AuthorizationServiceEnums.CourierCompanyStatusEnums.PendingApproval;
    public virtual User OwnerUser { get; set; }
    public virtual ICollection<CourierCompanyMember> Members { get; set; }
}
