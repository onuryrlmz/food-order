namespace Domain.Dto.Buyer.Order;

public class PlaceOrderRequestDto
{
    public Guid RestaurantId { get; set; }
    public Guid DeliveryAddressId { get; set; }
    public Guid? InvoiceAddressId { get; set; }
    public int PaymentOptionId { get; set; }
    public string? Notes { get; set; }
    public List<PlaceOrderItemDto> Items { get; set; } = new();
}

public class PlaceOrderItemDto
{
    public Guid MenuId { get; set; }
    public int Quantity { get; set; }
    public List<PlaceOrderItemValueDto> Values { get; set; } = new();
}

public class PlaceOrderItemValueDto
{
    public Guid MenuOptionId { get; set; }
    public Guid MenuOptionValueId { get; set; }
    public Guid? ProductId { get; set; }
    public int Quantity { get; set; }
    public List<PlaceOrderItemValueOptionDto> Options { get; set; } = new();
}

public class PlaceOrderItemValueOptionDto
{
    public Guid MenuOptionValueOptionId { get; set; }
    public Guid MenuOptionValueOptionValueId { get; set; }
    public Guid? ProductId { get; set; }
    public int Quantity { get; set; }
}
