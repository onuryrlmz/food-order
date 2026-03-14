namespace Domain.Dto.Seller.Restaurant;

public class GetRestaurantsRequestDto
{
    public Guid AddressId { get; set; }
    public double? Latitude { get; set; }
    public double? Longitude { get; set; }
}