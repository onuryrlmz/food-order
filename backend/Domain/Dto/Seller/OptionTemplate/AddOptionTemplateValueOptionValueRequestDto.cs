using NArchitecture.Core.Application.Dtos;

namespace Domain.Dto.Seller.OptionTemplate;

public class AddOptionTemplateValueOptionValueRequestDto : IDto
{
    public Guid OptionTemplateValueOptionId { get; set; }
    public Guid ProductId { get; set; }
    public decimal Price { get; set; }
}
