namespace Domain.Dto.Seller.Restaurant;

public class GetRestaurantsRequestDto
{
    public Guid AddressId { get; set; }
    public double? Latitude { get; set; }
    public double? Longitude { get; set; }

    // Search & Filter
    public string? Search { get; set; }
    public Guid? CuisineId { get; set; }
    public decimal? MinRating { get; set; }
    public decimal? MaxMinOrder { get; set; }
    public bool? IsOpen { get; set; }
    public string? SortBy { get; set; } // "rating", "minOrder", "deliveryTime", "name"
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 50;
}