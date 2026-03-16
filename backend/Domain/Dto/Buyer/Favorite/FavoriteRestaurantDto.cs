namespace Domain.Dto.Buyer.Favorite;

public class FavoriteRestaurantDto
{
    public Guid Id { get; set; }
    public Guid RestaurantId { get; set; }
    public string RestaurantName { get; set; }
    public string? CoverImage { get; set; }
    public decimal Rating { get; set; }
    public int RatingCount { get; set; }
    public decimal MinimumOrderPrice { get; set; }
    public int MinDeliveryTime { get; set; }
    public int MaxDeliveryTime { get; set; }
    public bool IsOpen { get; set; }
    public DateTime CreatedDate { get; set; }
}
