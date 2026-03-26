using Domain.Entities.Common;
using NArchitecture.Core.Persistence.Repositories;

namespace Domain.Entities.Buyer;

public class Notification : Entity<Guid>
{
    public Guid UserId { get; set; }
    public short TypeId { get; set; }
    public string Title { get; set; }
    public string Message { get; set; }
    public string? Data { get; set; }
    public bool IsRead { get; set; }
    public DateTime? ReadAt { get; set; }
    public Guid? RelatedOrderId { get; set; }
    public virtual User User { get; set; }
}
