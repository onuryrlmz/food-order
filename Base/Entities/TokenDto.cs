using Base.Enums;

namespace Base.Entities;

public class TokenDto : IDto
{
    public Guid UserId { get; set; }
    public Guid Token { get; set; }
    public DateTime Expiration { get; set; }
    public AuthorizationServiceEnums.UserRoleEnums Role { get; set; }

    // For Seller Properties
    public Guid? SellerId { get; set; }
    public List<Guid>? RestaurantIds { get; set; }
}