namespace Domain.Dto.Buyer.Support;

public class SupportTicketDto
{
    public Guid Id { get; set; }
    public string Subject { get; set; }
    public short TopicId { get; set; }
    public short StatusId { get; set; }
    public bool IsEscalated { get; set; }
    public short? Rating { get; set; }
    public DateTime CreatedDate { get; set; }
    public DateTime? ResolvedAt { get; set; }
    public int MessageCount { get; set; }
}
