using Application.Services.Buyer.ScheduledOrderService;

namespace Application.Services.Common.BackgroundJobs;

public class ScheduledOrderJobService : IScheduledOrderJobService
{
    private readonly IScheduledOrderService _scheduledOrderService;

    public ScheduledOrderJobService(IScheduledOrderService scheduledOrderService)
    {
        _scheduledOrderService = scheduledOrderService;
    }

    public async Task ProcessDueScheduledOrders()
    {
        await _scheduledOrderService.ProcessDueScheduledOrders();
    }
}
