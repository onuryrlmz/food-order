namespace Domain.Dto.Buyer.Review;

public class ReviewDto
{
    public Guid Id { get; set; }
    public Guid OrderId { get; set; }
    public Guid UserId { get; set; }
    public Guid RestaurantId { get; set; }
    public short Rating { get; set; }
    public string? Comment { get; set; }
    public string UserFirstName { get; set; }
    public string UserLastName { get; set; }
    public DateTime CreatedDate { get; set; }
}

public class CreateReviewRequestDto
{
    public Guid OrderId { get; set; }
    public short Rating { get; set; }
    public string? Comment { get; set; }
}
