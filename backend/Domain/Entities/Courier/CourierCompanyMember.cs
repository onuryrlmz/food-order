using Domain.Entities.Common;
using NArchitecture.Core.Persistence.Repositories;

namespace Domain.Entities.Courier;

public class CourierCompanyMember : Entity<Guid>
{
    public Guid CourierCompanyId { get; set; }
    public Guid CourierId { get; set; }
    public short StatusId { get; set; } = (short)Base.Enums.AuthorizationServiceEnums.CourierCompanyMemberStatusEnums.PendingApproval;
    public DateTime RequestedAt { get; set; } = DateTime.UtcNow;
    public DateTime? ApprovedAt { get; set; }
    public virtual CourierCompany CourierCompany { get; set; }
    public virtual User CourierUser { get; set; }
}
