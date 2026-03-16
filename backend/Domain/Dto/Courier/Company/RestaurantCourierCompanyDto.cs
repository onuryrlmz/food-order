using Base.Entities;

namespace Domain.Dto.Courier.Company;

public class RestaurantCourierCompanyDto : IDto
{
    public Guid Id { get; set; }
    public Guid CourierCompanyId { get; set; }
    public string CompanyName { get; set; }
    public short StatusId { get; set; }
    public string StatusName { get; set; }
    public DateTime? AgreementStartDate { get; set; }
}
