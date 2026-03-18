using NArchitecture.Core.Persistence.Repositories;

namespace Domain.Entities.Buyer;

public class Order : Entity<Guid>
{
    public Guid UserId { get; set; }
    public Guid SellerId { get; set; }
    public Guid RestaurantId { get; set; }
    public Guid DeliveryAddressId { get; set; }
    public Guid? InvoiceAddressId { get; set; }
    public short StatusId { get; set; }
    public short PaymentStatusId { get; set; }
    public int PaymentOptionId { get; set; }
    public decimal TotalProductPrice { get; set; }
    public decimal ShipmentPrice { get; set; }
    public decimal DiscountAmount { get; set; }
    public decimal TotalPrice { get; set; }
    
    // Coupon
    public Guid? CouponId { get; set; }
    public string? CouponCode { get; set; }
    
    public string? Notes { get; set; }
    public string? CancellationReason { get; set; }

    public DateTime? PickedUpAt { get; set; }
    public DateTime? DeliveredAt { get; set; }
    public decimal? DeliveryDistanceKm { get; set; }

    public virtual ICollection<OrderItem> OrderItems { get; set; }
    public virtual ICollection<Payment> Payments { get; set; }
    public virtual ICollection<OrderStatusHistory> StatusHistory { get; set; }
}
