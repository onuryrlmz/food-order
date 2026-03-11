namespace Domain.Dto.Seller.Product;

public class GetProductsRequestDto
{
    public Guid RestaurantId { get; set; }
    public List<Guid>? ProductIds { get; set; }
    public bool GetDetails { get; set; }
}