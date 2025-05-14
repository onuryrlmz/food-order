namespace Domain.Dto.Seller.Menu;

public class CreateMenuRequestDto
{
    public Guid RestaurantId { get; set; }
    public string Name { get; set; }
    public string? Description { get; set; }
    public double Price { get; set; }
    public int OrderIndex { get; set; }
}