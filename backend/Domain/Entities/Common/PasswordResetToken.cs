using NArchitecture.Core.Persistence.Repositories;

namespace Domain.Entities.Common;

public class PasswordResetToken : Entity<Guid>
{
    public Guid UserId { get; set; }
    public string Code { get; set; }
    public short Method { get; set; } // Email=1, Sms=2
    public DateTime ExpiresAt { get; set; }
    public DateTime? UsedAt { get; set; }
    public int FailedAttempts { get; set; } = 0;
    public virtual User User { get; set; }
}