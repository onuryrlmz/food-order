using NArchitecture.Core.Persistence.Repositories;

namespace Domain.Entities.Seller;

public class CategoryDetail : Entity<Guid>
{
    public Guid CategoryId { get; set; }

    public Guid MenuId { get; set; }
    public int OrderIndex { get; set; }

    public virtual Category Category { get; set; }
}