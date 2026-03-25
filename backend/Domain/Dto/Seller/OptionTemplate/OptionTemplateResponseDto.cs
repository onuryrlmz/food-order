using Domain.Dto.Seller.Product;
using NArchitecture.Core.Application.Dtos;
using Newtonsoft.Json;

namespace Domain.Dto.Seller.OptionTemplate;

public class OptionTemplateResponseDto : IDto
{
    public OptionTemplateResponseDto()
    {
        OptionTemplateValues = [];
    }

    [JsonProperty(Order = 1)]
    public Guid Id { get; set; }

    [JsonProperty(Order = 2)]
    public Guid RestaurantId { get; set; }

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
    public List<OptionTemplateValueResponseDto> OptionTemplateValues { get; set; }
}

public class OptionTemplateValueResponseDto : IDto
{
    public OptionTemplateValueResponseDto()
    {
        OptionTemplateValueOptions = [];
    }

    [JsonProperty(Order = 1)]
    public Guid Id { get; set; }

    [JsonProperty(Order = 2)]
    public decimal Price { get; set; }

    [JsonProperty(Order = 3)]
    public int OrderIndex { get; set; }

    [JsonProperty(Order = 4)]
    public ProductResponseDto Product { get; set; }

    [JsonProperty(Order = 5)]
    public List<OptionTemplateValueOptionResponseDto> OptionTemplateValueOptions { get; set; }

    [JsonIgnore]
    public Guid ProductId { get; set; }
}

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

public class OptionTemplateValueOptionValueResponseDto : IDto
{
    [JsonProperty(Order = 1)]
    public Guid Id { get; set; }

    [JsonProperty(Order = 2)]
    public decimal Price { get; set; }

    [JsonProperty(Order = 3)]
    public int OrderIndex { get; set; }

    [JsonProperty(Order = 4)]
    public ProductResponseDto Product { get; set; }

    [JsonIgnore]
    public Guid ProductId { get; set; }
}