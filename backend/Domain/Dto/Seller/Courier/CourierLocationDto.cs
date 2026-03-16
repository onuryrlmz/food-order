using Base.Entities;

namespace Domain.Dto.Seller.Courier;

public class CourierLocationDto : IDto
{
    public decimal Latitude { get; set; }
    public decimal Longitude { get; set; }
    public DateTime UpdatedAt { get; set; }
}
