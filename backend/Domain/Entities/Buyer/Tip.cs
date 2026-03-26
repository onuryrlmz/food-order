using NArchitecture.Core.Persistence.Repositories;

namespace Domain.Entities.Buyer;

public class Tip : Entity<Guid>
{
    public Guid OrderId { get; set; }
    public Guid UserId { get; set; }
    public Guid? CourierId { get; set; }
    public decimal Amount { get; set; }
    public short? PresetPercentage { get; set; }
    public bool IsPreDelivery { get; set; }
    public bool IsSettled { get; set; }
    public virtual Order Order { get; set; }
}
