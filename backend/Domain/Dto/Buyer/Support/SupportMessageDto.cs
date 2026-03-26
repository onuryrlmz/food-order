namespace Domain.Dto.Buyer.Support;

public class SupportMessageDto
{
    public Guid Id { get; set; }
    public short SenderType { get; set; }
    public string Content { get; set; }
    public DateTime CreatedDate { get; set; }
}
