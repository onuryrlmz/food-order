namespace Domain.Dto.Buyer.ScheduledOrder;

public class ScheduledOrderDto
{
    public Guid Id { get; set; }
    public Guid RestaurantId { get; set; }
    public string? RestaurantName { get; set; }
    public short StatusId { get; set; }
    public DateTime ScheduledDeliveryTime { get; set; }
    public DateTime ProcessAt { get; set; }
    public string? Notes { get; set; }
    public Guid? ConvertedOrderId { get; set; }
    public DateTime CreatedDate { get; set; }
}
