using NArchitecture.Core.Persistence.Repositories;

namespace Domain.Entities.Seller;

public class RestaurantWorkingHour : Entity<Guid>
{
    public Guid RestaurantId { get; set; }
    public short DayOfWeek { get; set; } // 1=Pazartesi … 7=Pazar
    public TimeOnly OpenTime { get; set; }
    public TimeOnly CloseTime { get; set; }
    public bool IsClosed { get; set; } = false;

    public virtual Restaurant Restaurant { get; set; }
}