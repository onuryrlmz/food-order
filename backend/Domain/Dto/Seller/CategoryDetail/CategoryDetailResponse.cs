using NArchitecture.Core.Application.Dtos;
using Newtonsoft.Json;

namespace Domain.Dto.Seller.CategoryDetail;

public class CategoryDetailResponse : IDto
{
    [JsonProperty(Order = 1)]
    public Guid Id { get; set; }

    [JsonProperty(Order = 2)]
    public Guid CategoryId { get; set; }

    [JsonProperty(Order = 3)]
    public Guid MenuId { get; set; }

    [JsonProperty(Order = 4)]
    public int OrderIndex { get; set; }
}