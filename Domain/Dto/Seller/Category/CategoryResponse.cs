using Domain.Dto.Seller.CategoryDetail;
using Domain.Dto.Seller.Menu;
using NArchitecture.Core.Application.Dtos;
using Newtonsoft.Json;

namespace Domain.Dto.Seller.Category;

public class CategoryResponse : IDto
{
    public CategoryResponse()
    {
        CategoryDetails = [];
        Menus = [];
    }

    [JsonProperty(Order = 1)]
    public Guid Id { get; set; }

    [JsonProperty(Order = 2)]
    public string Name { get; set; }

    [JsonProperty(Order = 3)]
    public int OrderIndex { get; set; }

    [JsonProperty(Order = 4)]
    public List<MenuResponseDto> Menus { get; set; }

    [JsonIgnore]
    public List<CategoryDetailResponse> CategoryDetails { get; set; }
}