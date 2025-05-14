using Base.Entities;

namespace Domain.Dto.Seller;

public class RestaurantResponseDto : IDto
{
    public RestaurantResponseDto()
    {
        //Categories = new List<CategoryResponse>();
    }

    public string Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }

    public string ImageUrl { get; set; }
    //public List<CategoryResponse> Categories { get; set; }
}