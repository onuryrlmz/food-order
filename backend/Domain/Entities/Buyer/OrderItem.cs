using NArchitecture.Core.Persistence.Repositories;

namespace Domain.Entities.Buyer;

public class OrderItem : Entity<Guid>
{
    public Guid OrderId { get; set; }
    public Guid MenuId { get; set; }
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal TotalPrice { get; set; }
    public string? ItemSnapshotJson { get; set; }

    public virtual Order Order { get; set; }
    public virtual ICollection<OrderItemValue> OrderItemValues { get; set; }
}