namespace Domain.Dto.Seller.ProductAttributeValue;

public class AddProductAttributeValueDto
{
    public Guid ProductAttributeId { get; set; }
    public Guid ProductId { get; set; }
    public double Price { get; set; }
}