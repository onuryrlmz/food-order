using NArchitecture.Core.Persistence.Repositories;

namespace Domain.Entities.Seller;

public class ProductImage : Entity<Guid>
{
    public Guid? ProductId { get; set; }
    public string Url { get; set; }
    public int OrderIndex { get; set; }
}