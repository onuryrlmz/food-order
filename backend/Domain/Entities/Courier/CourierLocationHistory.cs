using NArchitecture.Core.Persistence.Repositories;

namespace Domain.Entities.Courier;

public class CourierLocationHistory : Entity<Guid>
{
    public Guid CourierId { get; set; }
    public Guid? DeliveryAssignmentId { get; set; }
    public decimal Latitude { get; set; }
    public decimal Longitude { get; set; }
    public DateTime RecordedAt { get; set; }

    public virtual Courier Courier { get; set; }
    public virtual DeliveryAssignment? DeliveryAssignment { get; set; }
}
