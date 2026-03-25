namespace Domain.Dto.Courier;

public class UpdateCourierProfileRequestDto
{
    public string? VehicleType { get; set; }
    public string? VehiclePlate { get; set; }
    public string? IBAN { get; set; }
}