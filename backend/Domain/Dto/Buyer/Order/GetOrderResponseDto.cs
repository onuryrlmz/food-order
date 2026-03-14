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
    public Guid? CouponId { get; set; }
    public string? CouponCode { get; set; }
    public string? Notes { get; set; }
    public string? CancellationReason { get; set; }
    public DateTime CreatedDate { get; set; }
    public List<GetOrderItemResponseDto> Items { get; set; } = new();
}

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

public class OrderItemValueDto
{
    public string OptionName { get; set; }
    public string ValueName { get; set; }
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal TotalPrice { get; set; }
    public List<OrderItemValueOptionDto> Options { get; set; } = new();
}

public class OrderItemValueOptionDto
{
    public string OptionName { get; set; }
    public string ValueName { get; set; }
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal TotalPrice { get; set; }
}
