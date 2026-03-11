namespace Domain.Dto.Seller.CategoryDetail;

public class DeleteCategoryDetailRequestDto
{
    public Guid Id { get; set; }
    public Guid RestaurantId { get; set; }
    public Guid CategoryId { get; set; }
}