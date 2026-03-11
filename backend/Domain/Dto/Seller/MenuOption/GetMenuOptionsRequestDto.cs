namespace Domain.Dto.Seller.MenuOption;

public class GetMenuOptionsRequestDto
{
    public Guid RestaurantId { get; set; }
    public Guid MenuId { get; set; }
}