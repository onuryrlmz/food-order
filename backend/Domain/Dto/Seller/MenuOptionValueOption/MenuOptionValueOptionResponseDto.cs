using Domain.Dto.Seller.MenuOptionValueOptionValue;
using NArchitecture.Core.Application.Dtos;
using Newtonsoft.Json;

namespace Domain.Dto.Seller.MenuOptionValueOption;

public class MenuOptionValueOptionResponseDto : IDto
{
    public MenuOptionValueOptionResponseDto()
    {
        MenuOptionValueOptionValues = [];
    }

    [JsonProperty(Order = 1)]
    public Guid Id { get; set; }

    [JsonProperty(Order = 2)]
    public Guid MenuOptionValueId { get; set; }

    [JsonProperty(Order = 3)]
    public string Name { get; set; }

    [JsonProperty(Order = 4)]
    public string? Description { get; set; }

    [JsonProperty(Order = 5)]
    public int MinCount { get; set; }

    [JsonProperty(Order = 6)]
    public int MaxCount { get; set; }

    [JsonProperty(Order = 7)]
    public int OrderIndex { get; set; }

    [JsonProperty(Order = 8)]
    public List<MenuOptionValueOptionValueResponseDto> MenuOptionValueOptionValues { get; set; }
}