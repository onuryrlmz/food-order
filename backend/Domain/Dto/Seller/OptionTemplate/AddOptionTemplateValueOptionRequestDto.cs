using NArchitecture.Core.Application.Dtos;

namespace Domain.Dto.Seller.OptionTemplate;

public class AddOptionTemplateValueOptionRequestDto : IDto
{
    public Guid OptionTemplateValueId { get; set; }
    public string Name { get; set; }
    public string? Description { get; set; }
    public int MinCount { get; set; }
    public int MaxCount { get; set; }
}
