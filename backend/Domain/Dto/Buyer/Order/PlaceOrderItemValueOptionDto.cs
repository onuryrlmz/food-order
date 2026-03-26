namespace Domain.Dto.Buyer.Order;

public class PlaceOrderItemValueOptionDto
{
    public Guid MenuOptionValueOptionId { get; set; }
    public Guid MenuOptionValueOptionValueId { get; set; }
    public Guid? ProductId { get; set; }
    public int Quantity { get; set; }
}
