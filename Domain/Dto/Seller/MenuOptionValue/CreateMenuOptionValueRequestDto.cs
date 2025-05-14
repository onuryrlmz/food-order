namespace Domain.Dto.Seller.MenuOptionValue;

public class CreateMenuOptionValueRequestDto
{
    public Guid MenuOptionId { get; set; }
    public Guid ProductId { get; set; }
    public double Price { get; set; }
    public int OrderIndex { get; set; }
}