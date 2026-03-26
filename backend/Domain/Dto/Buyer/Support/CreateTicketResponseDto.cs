namespace Domain.Dto.Buyer.Support;

public class CreateTicketResponseDto
{
    public Guid TicketId { get; set; }
    public short StatusId { get; set; }
    public DateTime CreatedDate { get; set; }
    public SupportMessageDto? AiResponse { get; set; }
}
