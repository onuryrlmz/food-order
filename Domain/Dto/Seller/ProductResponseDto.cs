using NArchitecture.Core.Application.Dtos;
using Newtonsoft.Json;

namespace Domain.Dto.Seller;

public class ProductResponseDto : IDto
{
    [JsonProperty(Order = 1)]
    public Guid Id { get; set; }

    [JsonProperty(Order = 2)]
    public string Name { get; set; }

    [JsonProperty(Order = 3)]
    public string? Description { get; set; }

    [JsonProperty(Order = 4)]
    public int ProductType { get; set; }

    [JsonProperty(Order = 5)]
    public int OrderIndex { get; set; }
}