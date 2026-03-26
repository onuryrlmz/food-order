using NArchitecture.Core.Application.Dtos;

namespace Domain.Dto.Seller.OptionTemplate;

public class DeleteOptionTemplateRequestDto : IDto
{
    public Guid Id { get; set; }
    public Guid RestaurantId { get; set; }
}
