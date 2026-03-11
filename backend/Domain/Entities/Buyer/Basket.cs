using NArchitecture.Core.Persistence.Repositories;

namespace Domain.Entities.Buyer;

public class Basket : Entity<Guid>
{
    public Guid UserId { get; set; }
    public Guid UserShippingAddressId { get; set; }
    public Guid UserInvoiceAddressId { get; set; }
    public Guid SellerId { get; set; }
    public Guid RestaurantId { get; set; }
    public int StatusId { get; set; }
    public int PaymentOptionId { get; set; }
    public int TotalQuantity { get; set; }
    public decimal TotalProductPrice { get; set; }
    public decimal TotalShipmentPrice { get; set; }
    public decimal TotalShipmentDiscount { get; set; }
    public decimal TotalDiscount { get; set; }
    public decimal TotalPrice { get; set; }

    public virtual ICollection<BasketItem> BasketItems { get; set; }
}
