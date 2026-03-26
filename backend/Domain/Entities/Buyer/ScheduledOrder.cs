using NArchitecture.Core.Persistence.Repositories;

namespace Domain.Entities.Buyer;

public class ScheduledOrder : Entity<Guid>
{
    public Guid UserId { get; set; }
    public Guid RestaurantId { get; set; }
    public Guid DeliveryAddressId { get; set; }
    public Guid? InvoiceAddressId { get; set; }
    public short StatusId { get; set; }
    public DateTime ScheduledDeliveryTime { get; set; }
    public DateTime ProcessAt { get; set; }
    public short PaymentOptionId { get; set; }
    public string? Notes { get; set; }
    public string? CancellationReason { get; set; }
    public Guid? ConvertedOrderId { get; set; }
    public string BasketSnapshotJson { get; set; }
    public Guid? CouponId { get; set; }
    public string? CouponCode { get; set; }
}
