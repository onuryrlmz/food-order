namespace Domain.Dto.Buyer.Order;

public class PlaceOrderItemDto
{
    public Guid MenuId { get; set; }
    public int Quantity { get; set; }
    public List<PlaceOrderItemValueDto> Values { get; set; } = new();
}
