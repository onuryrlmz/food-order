using NArchitecture.Core.Application.Dtos;

namespace Domain.Dto.Seller.OptionTemplate;

public class DeleteOptionTemplateValueOptionRequestDto : IDto
{
    public Guid Id { get; set; }
}
