namespace Domain.Dto.Buyer.Order;

public class PlaceOrderRequestDto
{
    public Guid RestaurantId { get; set; }
    public Guid DeliveryAddressId { get; set; }
    public Guid? InvoiceAddressId { get; set; }
    public int PaymentOptionId { get; set; }
    public string? Notes { get; set; }
    public Guid? CouponId { get; set; }
    public string? CouponCode { get; set; }
    public List<PlaceOrderItemDto> Items { get; set; } = new();

    // Online ödeme - yeni kart
    public string? CardHolderName { get; set; }
    public string? CardNumber { get; set; }
    public string? ExpireMonth { get; set; }
    public string? ExpireYear { get; set; }
    public string? Cvc { get; set; }
    public bool SaveCard { get; set; }
    public string? CardAlias { get; set; }

    // Online ödeme - kayıtlı kart
    public string? CardToken { get; set; }
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