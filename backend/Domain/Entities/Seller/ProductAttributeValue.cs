using NArchitecture.Core.Persistence.Repositories;

namespace Domain.Entities.Seller;

public class ProductAttributeValue : Entity<Guid>
{
    public Guid ProductAttributeId { get; set; }
    public Guid ProductId { get; set; }

    public virtual Product Product { get; set; }
    public virtual ProductAttribute ProductAttribute { get; set; }
}