namespace Domain.Dto.Seller.ProductAttributeValue;

public class GetProductAttributeValueResponseDto
{
    public Guid Id { get; set; }
    public Guid ProductId { get; set; }
    public Guid ProductAttributeId { get; set; }
}