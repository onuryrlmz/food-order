namespace Domain.Dto.Seller.Restaurant;

public class WorkingHourDto
{
    public Guid Id { get; set; }
    public Guid RestaurantId { get; set; }
    public short DayOfWeek { get; set; }
    public TimeOnly OpenTime { get; set; }
    public TimeOnly CloseTime { get; set; }
    public bool IsClosed { get; set; }
}

public class UpsertWorkingHourDto
{
    public Guid RestaurantId { get; set; }
    public short DayOfWeek { get; set; }
    public TimeOnly OpenTime { get; set; }
    public TimeOnly CloseTime { get; set; }
    public bool IsClosed { get; set; }
}
