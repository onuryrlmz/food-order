using NArchitecture.Core.Application.Dtos;

namespace Domain.Dto.Seller.OptionTemplate;

public class UpdateOptionTemplateRequestDto : IDto
{
    public Guid Id { get; set; }
    public Guid RestaurantId { get; set; }
    public string Name { get; set; }
    public string? Description { get; set; }
    public int MinCount { get; set; }
    public int MaxCount { get; set; }
}
