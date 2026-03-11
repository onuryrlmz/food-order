namespace Domain.Dto.Seller.CategoryDetail;

public class GetCategoryDetailsByCategoryIdRequestDto
{
    public Guid RestaurantId { get; set; }
    public Guid CategoryId { get; set; }
}