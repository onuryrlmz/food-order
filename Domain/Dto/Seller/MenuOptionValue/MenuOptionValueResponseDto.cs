using Domain.Dto.Seller.MenuOptionValueOption;
using NArchitecture.Core.Application.Dtos;
using Newtonsoft.Json;

namespace Domain.Dto.Seller.MenuOptionValue;

public class MenuOptionValueResponseDto : IDto
{
    public MenuOptionValueResponseDto()
    {
        MenuOptionValueOptions = new List<MenuOptionValueOptionResponseDto>();
    }

    [JsonProperty(Order = 1)]
    public Guid Id { get; set; }

    [JsonProperty(Order = 2)]
    public double Price { get; set; }

    [JsonProperty(Order = 3)]
    public int OrderIndex { get; set; }

    [JsonProperty(Order = 4)]
    public ProductResponseDto Product { get; set; }

    [JsonProperty(Order = 5)]
    public List<MenuOptionValueOptionResponseDto> MenuOptionValueOptions { get; set; }

    [JsonIgnore]
    public Guid ProductId { get; set; }
}