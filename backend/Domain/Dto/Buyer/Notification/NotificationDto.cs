namespace Domain.Dto.Buyer.Notification;

public class NotificationDto
{
    public Guid Id { get; set; }
    public short TypeId { get; set; }
    public string Title { get; set; }
    public string Message { get; set; }
    public string? Data { get; set; }
    public bool IsRead { get; set; }
    public DateTime CreatedDate { get; set; }
}
