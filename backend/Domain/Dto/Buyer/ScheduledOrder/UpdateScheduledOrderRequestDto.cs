namespace Domain.Dto.Buyer.ScheduledOrder;

public class UpdateScheduledOrderRequestDto
{
    public DateTime? ScheduledDeliveryTime { get; set; }
    public string? Notes { get; set; }
    public Guid? DeliveryAddressId { get; set; }
}
