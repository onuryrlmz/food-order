namespace Domain.Dto.Seller;

public class CreateProductDto
{
    public Guid RestaurantId { get; set; }
    public Guid? CuisineId { get; set; }
    public string Name { get; set; }
    public int ProductType { get; set; }
    public string? Description { get; set; }
    public int OrderIndex { get; set; }
}