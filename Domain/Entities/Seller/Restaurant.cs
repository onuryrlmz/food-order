using NArchitecture.Core.Persistence.Repositories;

namespace Domain.Entities.Seller;

public class Restaurant : Entity<Guid>
{
    public Guid SellerId { get; set; }

    public string Name { get; set; }
    public int OrderIndex { get; set; }
}