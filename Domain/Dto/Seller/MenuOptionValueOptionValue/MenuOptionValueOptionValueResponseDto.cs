using Domain.Dto.Seller.Product;
using NArchitecture.Core.Application.Dtos;
using Newtonsoft.Json;

namespace Domain.Dto.Seller.MenuOptionValueOptionValue;

public class MenuOptionValueOptionValueResponseDto : IDto
{
    [JsonProperty(Order = 1)]
    public Guid Id { get; set; }

    [JsonProperty(Order = 2)]
    public double Price { get; set; }

    [JsonProperty(Order = 3)]
    public int OrderIndex { get; set; }

    [JsonProperty(Order = 4)]
    public ProductResponseDto Product { get; set; }

    [JsonIgnore]
    public Guid ProductId { get; set; }
}