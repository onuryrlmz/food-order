using Domain.Entities.Seller;
using NArchitecture.Core.Persistence.Repositories;

namespace Domain.Entities.Buyer;

public class BasketItem : Entity<Guid>
{
    public Guid BasketId { get; set; }
    public Guid MenuId { get; set; }
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal TotalPrice { get; set; }

    public virtual Basket Basket { get; set; }
    public virtual Menu Menu { get; set; }
    public virtual ICollection<BasketItemValue> BasketItemValues { get; set; }
}