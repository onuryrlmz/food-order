namespace Domain.Dto.Seller.ProductAttribute;

public class GetProductAttributeResponseDto
{
    public Guid Id { get; set; }
    public Guid ProductId { get; set; }
    public string Name { get; set; }
    public string? Description { get; set; }
    public int Type { get; set; }
    public int MinCount { get; set; }
    public int MaxCount { get; set; }
}