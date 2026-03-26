using NArchitecture.Core.Persistence.Repositories;

namespace Domain.Entities.Buyer;

public class SupportAction : Entity<Guid>
{
    public Guid TicketId { get; set; }
    public short ActionType { get; set; }
    public string ActionData { get; set; }
    public bool IsApproved { get; set; }
    public bool IsExecuted { get; set; }
    public Guid? ApprovedByUserId { get; set; }
    public DateTime? ExecutedAt { get; set; }
    public virtual SupportTicket Ticket { get; set; }
}
