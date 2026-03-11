namespace Domain.Dto.Seller.MenuOptionValueOption;

public class UpdateMenuOptionValueOptionRequestDto
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string? Description { get; set; }
    public int MinCount { get; set; }
    public int MaxCount { get; set; }
    public int OrderIndex { get; set; }
}