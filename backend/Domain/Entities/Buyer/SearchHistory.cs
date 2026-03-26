using Domain.Entities.Common;
using NArchitecture.Core.Persistence.Repositories;

namespace Domain.Entities.Buyer;

public class SearchHistory : Entity<Guid>
{
    public Guid UserId { get; set; }
    public string Query { get; set; }
    public string? SearchType { get; set; }
    public int ResultCount { get; set; }
    public virtual User User { get; set; }
}
