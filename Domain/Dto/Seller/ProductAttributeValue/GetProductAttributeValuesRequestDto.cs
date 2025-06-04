namespace Domain.Dto.Seller.ProductAttributeValue;

public class GetProductAttributeValuesRequestDto
{
    public Guid ProductId { get; set; }
    public Guid ProductAttributeId { get; set; }
}