namespace Domain.Dto.Seller.Menu;

public class GetMenuInformationByMenuIdRequestDto
{
    public Guid MenuId { get; set; }
    public Guid RestaurantId { get; set; }
}