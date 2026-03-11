using NArchitecture.Core.Persistence.Repositories;

namespace Domain.Entities.Seller;

public class ProductAttribute : Entity<Guid>
{
    public Guid ProductId { get; set; }
    public string Name { get; set; }
    public int Type { get; set; }
    public string? Description { get; set; }
    public int MinCount { get; set; }
    public int MaxCount { get; set; }

    public virtual Product Product { get; set; }
    public virtual ICollection<ProductAttributeValue> ProductAttributeValues { get; set; }
}