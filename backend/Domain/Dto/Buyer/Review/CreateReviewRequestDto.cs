namespace Domain.Dto.Buyer.Review;

public class CreateReviewRequestDto
{
    public Guid OrderId { get; set; }
    public short Rating { get; set; }
    public string? Comment { get; set; }
}
