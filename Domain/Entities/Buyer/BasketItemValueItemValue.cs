using Domain.Entities.Seller;
using NArchitecture.Core.Persistence.Repositories;

namespace Domain.Entities.Buyer;

public class BasketItemValueItemValue : Entity<Guid>
{
    public Guid BasketItemValueId { get; set; }
    public Guid MenuOptionValueOptionId { get; set; }
    public Guid MenuOptionValueOptionValueId { get; set; }
    public Guid ProductId { get; set; }
    public double Quantity { get; set; }
    public double UnitPrice { get; set; }
    public double TotalPrice { get; set; }

    public virtual BasketItemValue BasketItemValue { get; set; }
    public virtual MenuOptionValueOption MenuOptionValueOption { get; set; }
    public virtual MenuOptionValueOptionValue MenuOptionValueOptionValue { get; set; }
    public virtual Product Product { get; set; }
}