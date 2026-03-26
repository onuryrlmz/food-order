namespace Application.Services.Common.BackgroundJobs;

public interface IScheduledOrderJobService
{
    Task ProcessDueScheduledOrders();
}
