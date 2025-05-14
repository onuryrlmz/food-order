namespace Domain.Dto.Seller.MenuOptionValueOptionValue;

public class UpdateMenuOptionValueOptionValueRequestDto
{
    public Guid Id { get; set; }
    public Guid ProductId { get; set; }
    public double Price { get; set; }
    public int OrderIndex { get; set; }
}