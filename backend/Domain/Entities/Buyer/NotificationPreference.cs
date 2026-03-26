using Domain.Entities.Common;
using NArchitecture.Core.Persistence.Repositories;

namespace Domain.Entities.Buyer;

public class NotificationPreference : Entity<Guid>
{
    public Guid UserId { get; set; }
    public short NotificationTypeId { get; set; }
    public bool IsEnabled { get; set; }
    public virtual User User { get; set; }
}
