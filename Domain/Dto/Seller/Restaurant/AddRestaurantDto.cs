namespace Domain.Dto.Seller.Restaurant;

public class AddRestaurantDto
{
    public Guid SellerId { get; set; }
    public string Name { get; set; }
    public int OrderIndex { get; set; }
}