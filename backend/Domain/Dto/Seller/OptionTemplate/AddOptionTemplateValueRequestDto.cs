using NArchitecture.Core.Application.Dtos;

namespace Domain.Dto.Seller.OptionTemplate;

public class AddOptionTemplateValueRequestDto : IDto
{
    public Guid OptionTemplateId { get; set; }
    public Guid ProductId { get; set; }
    public decimal Price { get; set; }
}
