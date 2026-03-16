using Base.Entities;

namespace Domain.Dto.Courier;

public class CourierRestaurantDto : IDto
{
    public Guid RestaurantCourierId { get; set; }
    public Guid RestaurantId { get; set; }
    public string RestaurantName { get; set; }
    public short StatusId { get; set; }
    public string StatusName { get; set; }
    public DateTime? AgreementStartDate { get; set; }
    public DateTime CreatedDate { get; set; }
}
