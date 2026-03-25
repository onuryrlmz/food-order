namespace Application.Services.Common.BackgroundJobs;

public interface IDeliveryTimeoutJobService
{
    Task CheckDeliveryTimeouts();
}