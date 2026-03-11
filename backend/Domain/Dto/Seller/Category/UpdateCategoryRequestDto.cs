namespace Domain.Dto.Seller.Category;

public class UpdateCategoryRequestDto
{
    public Guid RestaurantId { get; set; }
    public Guid Id { get; set; }
    public string Name { get; set; }
    public int OrderIndex { get; set; }
}