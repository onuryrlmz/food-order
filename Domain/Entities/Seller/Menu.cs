using NArchitecture.Core.Persistence.Repositories;

namespace Domain.Entities.Seller;

public class Menu : Entity<Guid>
{
    public Guid RestaurantId { get; set; }
    public string Name { get; set; }
    public string? Description { get; set; }
    public double Price { get; set; }
    public int OrderIndex { get; set; }

    public virtual Restaurant Restaurant { get; set; }
    public virtual ICollection<MenuOption> MenuOptions { get; set; }
}