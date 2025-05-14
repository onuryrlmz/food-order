using Domain.Entities.Seller;
using NArchitecture.Core.Persistence.Repositories;

namespace Domain.Entities.Buyer;

public class BasketItemValue : Entity<Guid>
{
    public Guid BasketItemId { get; set; }
    public Guid MenuOptionId { get; set; }
    public Guid MenuOptionValueId { get; set; }
    public Guid ProductId { get; set; }
    public double Quantity { get; set; }
    public double UnitPrice { get; set; }
    public double TotalPrice { get; set; }

    public virtual BasketItem BasketItem { get; set; }
    public virtual MenuOption MenuOption { get; set; }
    public virtual MenuOptionValue MenuOptionValue { get; set; }
    public virtual Product Product { get; set; }
    public virtual ICollection<BasketItemValueItemValue> BasketItemValueItemValues { get; set; }
}