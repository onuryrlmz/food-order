using NArchitecture.Core.Application.Dtos;
using Newtonsoft.Json;

namespace Domain.Dto.Seller.OptionTemplate;

public class OptionTemplateValueOptionResponseDto : IDto
{
    public OptionTemplateValueOptionResponseDto()
    {
        OptionTemplateValueOptionValues = [];
    }

    [JsonProperty(Order = 1)]
    public Guid Id { get; set; }

    [JsonProperty(Order = 2)]
    public string Name { get; set; }

    [JsonProperty(Order = 3)]
    public string? Description { get; set; }

    [JsonProperty(Order = 4)]
    public int MinCount { get; set; }

    [JsonProperty(Order = 5)]
    public int MaxCount { get; set; }

    [JsonProperty(Order = 6)]
    public int OrderIndex { get; set; }

    [JsonProperty(Order = 7)]
    public List<OptionTemplateValueOptionValueResponseDto> OptionTemplateValueOptionValues { get; set; }
}
