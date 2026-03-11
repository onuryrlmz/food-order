using Domain.Dto.Seller.MenuOption;
using NArchitecture.Core.Application.Dtos;
using Newtonsoft.Json;

namespace Domain.Dto.Seller.Menu;

public class MenuResponseDto : IDto
{
    public MenuResponseDto()
    {
        MenuOptions = [];
    }

    [JsonProperty(Order = 1)]
    public Guid Id { get; set; }

    [JsonProperty(Order = 2)]
    public string Name { get; set; }

    [JsonProperty(Order = 3)]
    public string? Description { get; set; }

    [JsonProperty(Order = 4)]
    public decimal Price { get; set; }

    [JsonProperty(Order = 5)]
    public int OrderIndex { get; set; }

    [JsonProperty(Order = 6)]
    public List<MenuOptionResponseDto> MenuOptions { get; set; }
}