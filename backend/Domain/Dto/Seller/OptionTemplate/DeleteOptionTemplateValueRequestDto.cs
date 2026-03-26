using NArchitecture.Core.Application.Dtos;

namespace Domain.Dto.Seller.OptionTemplate;

public class DeleteOptionTemplateValueRequestDto : IDto
{
    public Guid Id { get; set; }
}
