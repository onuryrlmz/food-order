using Base.Entities;

namespace Domain.Dto.Courier;

public class UpdateLocationRequestDto : IDto
{
    public decimal Latitude { get; set; }
    public decimal Longitude { get; set; }
    public Guid? OrderId { get; set; }
}
