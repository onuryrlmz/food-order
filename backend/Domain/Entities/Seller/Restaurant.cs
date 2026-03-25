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

    // Location & Service Area
    public decimal? Latitude { get; set; }
    public decimal? Longitude { get; set; }
    public string? ServiceAreaPolygonWkt { get; set; }

    // Status
    public bool IsActive { get; set; } = false;
    public bool IsOpen { get; set; } = false;

    // Ratings
    public decimal Rating { get; set; } = 0;
    public int RatingCount { get; set; } = 0;

    // Courier
    public short? DefaultAssignmentStrategyId { get; set; }
    public bool HasOwnCouriers { get; set; } = false;

    // Navigation
    public virtual ICollection<Subscription> Subscriptions { get; set; }
}