namespace Domain.Dto.Buyer.Support;

public class CreateTicketRequestDto
{
    public Guid? OrderId { get; set; }
    public Guid? RestaurantId { get; set; }
    public short TopicId { get; set; }
    public string Subject { get; set; }
    public string InitialMessage { get; set; }
}
