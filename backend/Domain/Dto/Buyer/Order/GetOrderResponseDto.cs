namespace Domain.Dto.Buyer.Order;

public class GetOrderResponseDto
{
    public Guid Id { get; set; }
    public Guid RestaurantId { get; set; }
    public string RestaurantName { get; set; }
    public short StatusId { get; set; }
    public string StatusName { get; set; }
    public short PaymentStatusId { get; set; }
    public int PaymentOptionId { get; set; }
    public decimal TotalProductPrice { get; set; }
    public decimal ShipmentPrice { get; set; }
    public decimal DiscountAmount { get; set; }
    public decimal TotalPrice { get; set; }
    public string? Notes { get; set; }
    public string? CancellationReason { get; set; }
    public DateTime CreatedDate { get; set; }
    public DateTime? ConfirmedAt { get; set; }
    public DateTime? DeliveredAt { get; set; }
    public List<GetOrderItemResponseDto> Items { get; set; } = new();
}

public class GetOrderItemResponseDto
{
    public Guid Id { get; set; }
    public Guid MenuId { get; set; }
    public string MenuName { get; set; }
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal TotalPrice { get; set; }
}
