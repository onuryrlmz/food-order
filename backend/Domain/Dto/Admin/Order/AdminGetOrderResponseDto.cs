namespace Domain.Dto.Admin.Order;

public class AdminGetOrderResponseDto
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string? CustomerName { get; set; }
    public string? CustomerEmail { get; set; }
    public string? CustomerPhone { get; set; }
    public Guid RestaurantId { get; set; }
    public string RestaurantName { get; set; }
    public Guid SellerId { get; set; }
    public short StatusId { get; set; }
    public string StatusName { get; set; }
    public short PaymentStatusId { get; set; }
    public string PaymentStatusName { get; set; }
    public int PaymentOptionId { get; set; }
    public string PaymentOptionName { get; set; }
    public decimal TotalProductPrice { get; set; }
    public decimal ShipmentPrice { get; set; }
    public decimal DiscountAmount { get; set; }
    public decimal TotalPrice { get; set; }
    public Guid? CouponId { get; set; }
    public string? CouponCode { get; set; }
    public string? Notes { get; set; }
    public string? CancellationReason { get; set; }
    public string? DeliveryAddress { get; set; }
    public DateTime CreatedDate { get; set; }
    public List<AdminGetOrderItemDto> Items { get; set; } = new();
    public List<AdminPaymentDto> Payments { get; set; } = new();
    public List<AdminStatusHistoryDto> StatusHistory { get; set; } = new();
}
