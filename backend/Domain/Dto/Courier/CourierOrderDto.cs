using Base.Entities;

namespace Domain.Dto.Courier;

public class CourierOrderDto : IDto
{
    public Guid OrderId { get; set; }
    public string RestaurantName { get; set; }
    public string DeliveryArea { get; set; }
    public decimal? DeliveryDistanceKm { get; set; }
    public short StatusId { get; set; }
    public string StatusName { get; set; }
    public DateTime CreatedDate { get; set; }
}
