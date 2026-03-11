namespace Domain.Dto.Seller.Product;

public class AddProductDto
{
    public Guid RestaurantId { get; set; }
    public Guid? CuisineId { get; set; }
    public string Name { get; set; }
    public int ProductType { get; set; }
    public string? Description { get; set; }
    public decimal Price { get; set; }
    public int OrderIndex { get; set; }
}