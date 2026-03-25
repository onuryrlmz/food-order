using NArchitecture.Core.Persistence.Repositories;

namespace Domain.Entities.Buyer;

public class OrderStatusHistory : Entity<Guid>
{
    public Guid OrderId { get; set; }
    public short StatusId { get; set; }
    public string? Note { get; set; }
    public DateTime OccurredAt { get; set; }

    public virtual Order Order { get; set; }
}