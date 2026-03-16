using Base.Enums;
using Domain.Entities.Common;
using NArchitecture.Core.Persistence.Repositories;

namespace Domain.Entities.Courier;

public class RestaurantCourier : Entity<Guid>
{
    public Guid RestaurantId { get; set; }
    public Guid CourierId { get; set; }
    public short StatusId { get; set; } = (short)AuthorizationServiceEnums.RestaurantCourierStatusEnums.PendingApproval;
    public DateTime? AgreementStartDate { get; set; }
    public DateTime? AgreementEndDate { get; set; }
    public virtual User CourierUser { get; set; }
}
