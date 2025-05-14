using NArchitecture.Core.Persistence.Repositories;

namespace Domain.Entities.Seller;

public class MenuOptionValue : Entity<Guid>
{
    public Guid MenuOptionId { get; set; }
    public Guid ProductId { get; set; }
    public double Price { get; set; }
    public int OrderIndex { get; set; }

    public virtual Product Product { get; set; }
    public virtual MenuOption MenuOption { get; set; }
    public virtual ICollection<MenuOptionValueOption> MenuOptionValueOptions { get; set; }
}