using NArchitecture.Core.Application.Dtos;

namespace Domain.Dto.Seller.OptionTemplate;

public class DeleteOptionTemplateValueOptionValueRequestDto : IDto
{
    public Guid Id { get; set; }
}
