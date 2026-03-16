using Base.Entities;

namespace Domain.Dto.Courier.Company;

public class PickupOrderDto : IDto
{
    public Guid OrderId { get; set; }
    public string RestaurantName { get; set; }
    public string DeliveryAddress { get; set; }
    public double? DeliveryLatitude { get; set; }
    public double? DeliveryLongitude { get; set; }
    public decimal? DeliveryDistanceKm { get; set; }
    public decimal TotalPrice { get; set; }
    public DateTime CreatedDate { get; set; }
}
