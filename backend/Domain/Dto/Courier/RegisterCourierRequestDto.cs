namespace Domain.Dto.Courier;

public class RegisterCourierRequestDto
{
    public short CourierTypeId { get; set; }
    public Guid? CourierCompanyId { get; set; }
    public Guid? RestaurantId { get; set; }
    public string? VehicleType { get; set; }
    public string? VehiclePlate { get; set; }
    public string? IdentityNumber { get; set; }
    public string? IBAN { get; set; }
}