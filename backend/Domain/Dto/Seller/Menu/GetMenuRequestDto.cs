using Domain.Service;
using MediatR;

namespace Domain.Dto.Seller.Menu;

public class GetMenuRequestDto : IRequest<ServiceObjectResult<bool>>
{
    public Guid MenuId { get; set; }
    public Guid RestaurantId { get; set; }
}