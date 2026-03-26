namespace Domain.Dto.Buyer.ScheduledOrder;

public class CreateScheduledOrderRequestDto
{
    public Guid RestaurantId { get; set; }
    public Guid DeliveryAddressId { get; set; }
    public Guid? InvoiceAddressId { get; set; }
    public DateTime ScheduledDeliveryTime { get; set; }
    public short PaymentOptionId { get; set; }
    public string? Notes { get; set; }
    public string? CouponCode { get; set; }
    public string BasketSnapshotJson { get; set; }
}
