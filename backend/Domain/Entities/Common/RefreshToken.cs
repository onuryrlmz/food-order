using NArchitecture.Core.Persistence.Repositories;

namespace Domain.Entities.Common;

public class RefreshToken : Entity<Guid>
{
    public Guid UserId { get; set; }
    public string Token { get; set; }
    public DateTime ExpiresAt { get; set; }
    public DateTime? RevokedAt { get; set; }
    public string? ReplacedByToken { get; set; }
    public virtual User User { get; set; }
}
