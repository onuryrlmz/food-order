using Base.Entities;

namespace Domain.Dto.Seller.Courier;

public class AssignCourierRequestDto : IDto
{
    public Guid CourierId { get; set; }
}
