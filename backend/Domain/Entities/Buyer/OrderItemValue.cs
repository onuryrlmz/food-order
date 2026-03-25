using Domain.Entities.Seller;
using NArchitecture.Core.Persistence.Repositories;

namespace Domain.Entities.Buyer;

public class OrderItemValue : Entity<Guid>
{
    public Guid OrderItemId { get; set; }
    public Guid MenuOptionId { get; set; }
    public Guid MenuOptionValueId { get; set; }
    public Guid ProductId { get; set; }
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal TotalPrice { get; set; }

    public virtual OrderItem OrderItem { get; set; }
    public virtual MenuOption MenuOption { get; set; }
    public virtual MenuOptionValue MenuOptionValue { get; set; }
    public virtual Product Product { get; set; }
    public virtual ICollection<OrderItemValueOption> OrderItemValueOptions { get; set; }
}