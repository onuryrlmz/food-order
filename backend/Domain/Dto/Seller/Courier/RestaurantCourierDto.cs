using Base.Entities;

namespace Domain.Dto.Seller.Courier;

public class RestaurantCourierDto : IDto
{
    public Guid Id { get; set; }
    public Guid CourierId { get; set; }
    public string Email { get; set; }
    public string? PhoneNumber { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public short StatusId { get; set; }
    public string StatusName { get; set; }
    public short? CourierStatusId { get; set; }
    public string? CourierStatusName { get; set; }
    public DateTime? AgreementStartDate { get; set; }
    public DateTime? AgreementEndDate { get; set; }
    public DateTime CreatedDate { get; set; }
}
