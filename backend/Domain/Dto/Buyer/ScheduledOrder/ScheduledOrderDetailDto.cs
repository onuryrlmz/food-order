namespace Domain.Dto.Buyer.ScheduledOrder;

public class ScheduledOrderDetailDto
{
    public Guid Id { get; set; }
    public Guid RestaurantId { get; set; }
    public string? RestaurantName { get; set; }
    public short StatusId { get; set; }
    public DateTime ScheduledDeliveryTime { get; set; }
    public DateTime ProcessAt { get; set; }
    public short PaymentOptionId { get; set; }
    public string? Notes { get; set; }
    public string? CancellationReason { get; set; }
    public Guid? ConvertedOrderId { get; set; }
    public string BasketSnapshotJson { get; set; }
    public string? CouponCode { get; set; }
    public DateTime CreatedDate { get; set; }
}
