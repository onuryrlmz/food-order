using Base.Entities;

namespace Domain.Dto.Courier.Company;

public class SearchCourierRequestDto : IDto
{
    public string Query { get; set; }
}
