namespace Domain.Dto.Admin.Order;

public class AdminGetOrderItemDto
{
    public Guid Id { get; set; }
    public Guid MenuId { get; set; }
    public string MenuName { get; set; }
    public string? Description { get; set; }
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal TotalPrice { get; set; }
    public List<Buyer.Order.OrderItemValueDto> Values { get; set; } = new();
}
