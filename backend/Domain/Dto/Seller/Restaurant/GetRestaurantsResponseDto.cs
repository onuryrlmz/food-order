using Base.Entities;

namespace Domain.Dto.Seller.Restaurant;

public class GetRestaurantsResponseDto : IDto
{
    public GetRestaurantsResponseDto()
    {
        Categories = [];
    }

    public Guid Id { get; set; }
    public Guid SellerId { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public string ImageUrl { get; set; }
    public decimal MinBasketPrice { get; set; }
    public decimal DeliveryPrice { get; set; }
    public int MinDeliveryTime { get; set; }
    public int MaxDeliveryTime { get; set; }
    public List<string> Categories { get; set; }
}