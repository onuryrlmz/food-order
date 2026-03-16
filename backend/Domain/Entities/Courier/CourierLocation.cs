using Domain.Entities.Common;
using NArchitecture.Core.Persistence.Repositories;

namespace Domain.Entities.Courier;

public class CourierLocation : Entity<Guid>
{
    public Guid CourierId { get; set; }
    public Guid? OrderId { get; set; }
    public decimal Latitude { get; set; }
    public decimal Longitude { get; set; }
    public DateTime UpdatedAt { get; set; }
    public virtual User CourierUser { get; set; }
}
