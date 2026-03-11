namespace Domain.Dto.Seller.Menu;

public class DeleteMenuRequestDto
{
    public Guid Id { get; set; }
    public Guid RestaurantId { get; set; }
}