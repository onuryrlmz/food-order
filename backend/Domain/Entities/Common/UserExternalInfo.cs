using NArchitecture.Core.Persistence.Repositories;

namespace Domain.Entities.Common;

public class UserExternalInfo : Entity<Guid>
{
    public Guid UserId { get; set; }
    public string Provider { get; set; }
    public string Key { get; set; }
    public string Value { get; set; }

    public virtual User User { get; set; }
}
