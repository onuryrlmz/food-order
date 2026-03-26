using Domain.Entities.Common;
using NArchitecture.Core.Persistence.Repositories;

namespace Domain.Entities.Buyer;

public class SupportTicket : Entity<Guid>
{
    public Guid UserId { get; set; }
    public Guid? OrderId { get; set; }
    public Guid? RestaurantId { get; set; }
    public short TopicId { get; set; }
    public short StatusId { get; set; }
    public string Subject { get; set; }
    public bool IsEscalated { get; set; }
    public short? Rating { get; set; }
    public string? RatingComment { get; set; }
    public DateTime? ResolvedAt { get; set; }
    public DateTime? ClosedAt { get; set; }
    public virtual User User { get; set; }
    public virtual ICollection<SupportMessage> Messages { get; set; }
    public virtual ICollection<SupportAction> Actions { get; set; }
}
