namespace Domain.Dto.Seller.Category;

public class GetCategoriesByRestaurantIdRequestDto
{
    public Guid RestaurantId { get; set; }
    public bool GetMenus { get; set; }
    public bool GetMenuProducts { get; set; }
}