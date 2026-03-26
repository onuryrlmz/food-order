using NArchitecture.Core.Persistence.Repositories;

namespace Domain.Entities.Buyer;

public class SupportMessage : Entity<Guid>
{
    public Guid TicketId { get; set; }
    public short SenderType { get; set; }
    public string Content { get; set; }
    public string? AiModelUsed { get; set; }
    public int? TokensUsed { get; set; }
    public virtual SupportTicket Ticket { get; set; }
}
