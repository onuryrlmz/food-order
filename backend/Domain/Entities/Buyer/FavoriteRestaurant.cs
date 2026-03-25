using Domain.Entities.Common;
using Domain.Entities.Seller;
using NArchitecture.Core.Persistence.Repositories;

namespace Domain.Entities.Buyer;

public class FavoriteRestaurant : Entity<Guid>
{
    public Guid UserId { get; set; }
    public Guid RestaurantId { get; set; }
    public virtual User User { get; set; }
    public virtual Restaurant Restaurant { get; set; }
}