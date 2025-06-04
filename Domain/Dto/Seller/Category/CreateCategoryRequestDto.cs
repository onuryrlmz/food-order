namespace Domain.Dto.Seller.Category;

public class CreateCategoryRequestDto
{
    public Guid RestaurantId { get; set; }
    public string Name { get; set; }
    public int OrderIndex { get; set; }
}