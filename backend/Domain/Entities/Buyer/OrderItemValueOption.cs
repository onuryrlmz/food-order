using Domain.Entities.Seller;
using NArchitecture.Core.Persistence.Repositories;

namespace Domain.Entities.Buyer;

public class OrderItemValueOption : Entity<Guid>
{
    public Guid OrderItemValueId { get; set; }
    public Guid MenuOptionValueOptionId { get; set; }
    public Guid MenuOptionValueOptionValueId { get; set; }
    public Guid ProductId { get; set; }
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal TotalPrice { get; set; }

    public virtual OrderItemValue OrderItemValue { get; set; }
    public virtual MenuOptionValueOption MenuOptionValueOption { get; set; }
    public virtual MenuOptionValueOptionValue MenuOptionValueOptionValue { get; set; }
    public virtual Product Product { get; set; }
}