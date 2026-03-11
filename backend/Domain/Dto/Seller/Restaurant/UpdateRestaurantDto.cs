namespace Domain.Dto.Seller.Restaurant;

public class UpdateRestaurantDto
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Phone { get; set; }
    public string? Email { get; set; }
    public decimal MinimumOrderPrice { get; set; }
    public int MinDeliveryTime { get; set; }
    public int MaxDeliveryTime { get; set; }
    public string? Description { get; set; }
    public string? CoverImage { get; set; }
}
