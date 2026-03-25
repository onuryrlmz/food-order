namespace Domain.Dto.Courier;

public class CourierProfileResponseDto
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Phone { get; set; }
    public short CourierTypeId { get; set; }
    public short StatusId { get; set; }
    public short AvailabilityStatusId { get; set; }
    public string? VehicleType { get; set; }
    public string? VehiclePlate { get; set; }
    public decimal Rating { get; set; }
    public int RatingCount { get; set; }
    public int TotalDeliveries { get; set; }
    public Guid? CourierCompanyId { get; set; }
    public string? CourierCompanyName { get; set; }
    public Guid? RestaurantId { get; set; }
    public string? RestaurantName { get; set; }
}