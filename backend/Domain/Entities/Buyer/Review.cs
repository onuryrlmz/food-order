using Domain.Entities.Common;
using Domain.Entities.Seller;
using NArchitecture.Core.Persistence.Repositories;

namespace Domain.Entities.Buyer;

public class Review : Entity<Guid>
{
    public Guid OrderId { get; set; }
    public Guid UserId { get; set; }
    public Guid RestaurantId { get; set; }
    public short Rating { get; set; }
    public string? Comment { get; set; }
    public virtual User User { get; set; }
    public virtual Restaurant Restaurant { get; set; }
}