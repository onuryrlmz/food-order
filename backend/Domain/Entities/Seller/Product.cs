using NArchitecture.Core.Persistence.Repositories;

namespace Domain.Entities.Seller;

public class Product : Entity<Guid>
{
    public Guid RestaurantId { get; set; }
    public Guid CuisineId { get; set; }
    public string Name { get; set; }
    public int ProductType { get; set; }
    public decimal Price { get; set; }
    public string? Description { get; set; }
    public int OrderIndex { get; set; }

    public virtual Restaurant Restaurant { get; set; }
    public virtual ICollection<ProductAttribute> ProductAttributes { get; set; }
}