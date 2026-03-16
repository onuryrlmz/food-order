using Base.Entities;

namespace Domain.Dto.Courier.Company;

public class RequestMembershipDto : IDto
{
    public Guid CourierId { get; set; }
}
