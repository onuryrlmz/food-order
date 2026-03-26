namespace Domain.Dto.Buyer.Order;

public class OrderItemValueOptionDto
{
    public string OptionName { get; set; }
    public string ValueName { get; set; }
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal TotalPrice { get; set; }
}
