using Domain.Entities.Common;
using Domain.Entities.Seller;
using NArchitecture.Core.Persistence.Repositories;

namespace Domain.Entities.Courier;

public class Courier : Entity<Guid>
{
    public Guid UserId { get; set; }
    public Guid? CourierCompanyId { get; set; }
    public Guid? RestaurantId { get; set; }
    public short CourierTypeId { get; set; }
    public short StatusId { get; set; }
    public short AvailabilityStatusId { get; set; }
    public string? VehicleType { get; set; }
    public string? VehiclePlate { get; set; }
    public string? IdentityNumber { get; set; }
    public string? IBAN { get; set; }
    public decimal Rating { get; set; } = 0;
    public int RatingCount { get; set; } = 0;
    public int TotalDeliveries { get; set; } = 0;
    public decimal? CurrentLatitude { get; set; }
    public decimal? CurrentLongitude { get; set; }
    public DateTime? LastLocationUpdate { get; set; }

    public virtual User User { get; set; }
    public virtual CourierCompany? CourierCompany { get; set; }
    public virtual Restaurant? Restaurant { get; set; }
    public virtual ICollection<DeliveryAssignment> DeliveryAssignments { get; set; }
    public virtual ICollection<CourierEarning> Earnings { get; set; }
}