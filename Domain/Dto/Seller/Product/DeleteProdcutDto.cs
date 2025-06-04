namespace Domain.Dto.Seller.Product;

public class DeleteProductDto
{
    public Guid Id { get; set; }
    public Guid RestaurantId { get; set; }
}