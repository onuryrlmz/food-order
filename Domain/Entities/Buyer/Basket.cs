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
    public double TotalQuantity { get; set; }
    public double TotalProductPrice { get; set; }
    public double TotalShipmentPrice { get; set; }
    public double TotalShipmentDiscount { get; set; }
    public double TotalDiscount { get; set; }
    public double TotalPrice { get; set; }

    public virtual ICollection<BasketItem> BasketItems { get; set; }
}