namespace Domain.Dto.Buyer.Order;

public class OrderItemValueDto
{
    public string OptionName { get; set; }
    public string ValueName { get; set; }
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal TotalPrice { get; set; }
    public List<OrderItemValueOptionDto> Options { get; set; } = new();
}
