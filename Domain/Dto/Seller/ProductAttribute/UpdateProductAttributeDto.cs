namespace Domain.Dto.Seller.ProductAttribute;

public class UpdateProductAttributeDto
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public int Type { get; set; }
    public string? Description { get; set; }
    public int MinCount { get; set; }
    public int MaxCount { get; set; }
}