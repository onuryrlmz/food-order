using NArchitecture.Core.Persistence.Repositories;

namespace Domain.Entities.Seller;

public class ScheduledTask : Entity<Guid>
{
    public Guid RestaurantId { get; set; }

    public bool IsStart { get; set; }

    public bool IsFinish { get; set; }

    public int ScheduleJobId { get; set; }

    public Guid? ExtraId1 { get; set; }

    public Guid? ExtraId2 { get; set; }

    public Guid? ExtraId3 { get; set; }
    public int OrderIndex { get; set; }
}