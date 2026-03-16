using Domain.Entities.Common;
using Domain.Entities.Seller;
using NArchitecture.Core.Persistence.Repositories;

namespace Domain.Entities.Courier;

public class RestaurantCourierCompany : Entity<Guid>
{
    public Guid RestaurantId { get; set; }
    public Guid CourierCompanyId { get; set; }
    public short StatusId { get; set; } = (short)Base.Enums.AuthorizationServiceEnums.RestaurantCourierCompanyStatusEnums.PendingApproval;
    public DateTime? AgreementStartDate { get; set; }
    public DateTime? AgreementEndDate { get; set; }
    public virtual Restaurant Restaurant { get; set; }
    public virtual CourierCompany CourierCompany { get; set; }
}
