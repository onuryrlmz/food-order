namespace Domain.Dto.Buyer.Order;

public class PlaceOrderItemValueDto
{
    public Guid MenuOptionId { get; set; }
    public Guid MenuOptionValueId { get; set; }
    public Guid? ProductId { get; set; }
    public int Quantity { get; set; }
    public List<PlaceOrderItemValueOptionDto> Options { get; set; } = new();
}
