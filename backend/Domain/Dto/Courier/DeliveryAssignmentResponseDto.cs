namespace Domain.Dto.Courier;

public class DeliveryAssignmentResponseDto
{
    public Guid Id { get; set; }
    public Guid OrderId { get; set; }
    public Guid RestaurantId { get; set; }
    public string RestaurantName { get; set; }
    public string RestaurantPhone { get; set; }
    public decimal? RestaurantLatitude { get; set; }
    public decimal? RestaurantLongitude { get; set; }
    public decimal? CustomerLatitude { get; set; }
    public decimal? CustomerLongitude { get; set; }
    public short StatusId { get; set; }
    public decimal? DeliveryFee { get; set; }
    public decimal? DistanceKm { get; set; }
    public DateTime? OfferedAt { get; set; }
    public DateTime? AcceptedAt { get; set; }
    public DateTime? PickedUpAt { get; set; }
    public DateTime? DeliveredAt { get; set; }
    public DateTime? ExpiresAt { get; set; }
    public DateTime CreatedDate { get; set; }

    // Order info
    public decimal OrderTotalPrice { get; set; }
    public string? OrderNotes { get; set; }
    public string? CustomerAddress { get; set; }
    public string? CustomerName { get; set; }
    public string? CustomerPhone { get; set; }
}
