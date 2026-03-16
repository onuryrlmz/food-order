namespace Domain.Dto.Courier;

public class CourierEarningsSummaryDto
{
    public decimal TotalEarnings { get; set; }
    public int TotalDeliveries { get; set; }
    public decimal AveragePerDelivery { get; set; }
}

public class CourierEarningsHistoryDto
{
    public Guid OrderId { get; set; }
    public string RestaurantName { get; set; }
    public decimal ShipmentPrice { get; set; }
    public DateTime DeliveredAt { get; set; }
}
