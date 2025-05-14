namespace Domain.Dto.Seller.MenuOptionValueOptionValue;

public class CreateMenuOptionValueOptionValueRequestDto
{
    public Guid MenuOptionValueOptionId { get; set; }
    public Guid ProductId { get; set; }
    public double Price { get; set; }
    public int OrderIndex { get; set; }
}