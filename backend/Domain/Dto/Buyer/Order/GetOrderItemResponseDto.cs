namespace Domain.Dto.Buyer.Order;

public class GetOrderItemResponseDto
{
    public Guid Id { get; set; }
    public Guid MenuId { get; set; }
    public string MenuName { get; set; }
    public string? Description { get; set; }
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal TotalPrice { get; set; }
    public List<OrderItemValueDto> Values { get; set; } = new();
}
