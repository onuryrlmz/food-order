using NArchitecture.Core.Persistence.Repositories;

namespace Domain.Entities.Common;

public class Cuisine : Entity<Guid>
{
    public string Name { get; set; }
    public int OrderIndex { get; set; }
}