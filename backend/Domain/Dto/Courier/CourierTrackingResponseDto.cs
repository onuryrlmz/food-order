namespace Domain.Dto.Courier;

public class CourierTrackingResponseDto
{
    public Guid? CourierId { get; set; }
    public string? CourierName { get; set; }
    public string? CourierPhone { get; set; }
    public string? VehicleType { get; set; }
    public decimal? Latitude { get; set; }
    public decimal? Longitude { get; set; }
    public DateTime? LastLocationUpdate { get; set; }
    public short? AssignmentStatusId { get; set; }
}