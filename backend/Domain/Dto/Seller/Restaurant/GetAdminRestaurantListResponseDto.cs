namespace Domain.Dto.Seller.Restaurant;

public class GetAdminRestaurantListResponseDto
{
    public Guid Id { get; set; }
    public Guid SellerId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string? Email { get; set; }
    public string? Description { get; set; }
    public bool IsActive { get; set; }
    public bool IsOpen { get; set; }
    public decimal Rating { get; set; }
    public int RatingCount { get; set; }
    public decimal MinimumOrderPrice { get; set; }
    public int MinDeliveryTime { get; set; }
    public int MaxDeliveryTime { get; set; }
    public string? CoverImage { get; set; }
    public decimal? Latitude { get; set; }
    public decimal? Longitude { get; set; }
    public string? ServiceAreaPolygonWkt { get; set; }
    public DateTime CreatedDate { get; set; }
}