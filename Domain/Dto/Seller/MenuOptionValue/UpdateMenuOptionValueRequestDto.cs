namespace Domain.Dto.Seller.MenuOptionValue;

public class UpdateMenuOptionValueRequestDto
{
    public Guid Id { get; set; }
    public Guid ProductId { get; set; }
    public double Price { get; set; }
    public int OrderIndex { get; set; }
}