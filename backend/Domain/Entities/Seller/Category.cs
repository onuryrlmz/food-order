using NArchitecture.Core.Persistence.Repositories;

namespace Domain.Entities.Seller;

public class Category : Entity<Guid>
{
    public Guid RestaurantId { get; set; }

    public string Name { get; set; }
    public int OrderIndex { get; set; }

    public virtual Restaurant Restaurant { get; set; }
    public virtual ICollection<CategoryDetail> CategoryDetails { get; set; }
}