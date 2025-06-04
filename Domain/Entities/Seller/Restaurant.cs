using NArchitecture.Core.Persistence.Repositories;

namespace Domain.Entities.Seller;

public class Restaurant : Entity<Guid>
{
    public Guid SellerId { get; set; }
    public string Name { get; set; }
    public string Phone { get; set; }
    public string? Email { get; set; }
    public decimal MinimumOrderPrice { get; set; }
    public int MinDeliveryTime { get; set; }
    public int MaxDeliveryTime { get; set; }
    public string? CoverImage { get; set; }
    public string? Description { get; set; }
}