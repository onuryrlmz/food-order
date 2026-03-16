using Base.Entities;

namespace Domain.Dto.Courier.Company;

public class InviteCompanyRequestDto : IDto
{
    public Guid CompanyId { get; set; }
}
