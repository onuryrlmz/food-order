namespace Domain.Dto.Buyer.Support;

public class SupportTicketDetailDto
{
    public Guid Id { get; set; }
    public string Subject { get; set; }
    public short TopicId { get; set; }
    public short StatusId { get; set; }
    public bool IsEscalated { get; set; }
    public short? Rating { get; set; }
    public string? RatingComment { get; set; }
    public Guid? OrderId { get; set; }
    public Guid? RestaurantId { get; set; }
    public DateTime CreatedDate { get; set; }
    public DateTime? ResolvedAt { get; set; }
    public DateTime? ClosedAt { get; set; }
    public List<SupportMessageDto> Messages { get; set; }
    public List<SupportActionDto> Actions { get; set; }
}
