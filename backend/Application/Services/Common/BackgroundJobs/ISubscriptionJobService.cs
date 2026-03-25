namespace Application.Services.Common.BackgroundJobs;

public interface ISubscriptionJobService
{
    Task CheckExpiredSubscriptions();
    Task SendExpiryReminders();
    Task AutoRenewSubscriptions();
    Task CheckUsageWarnings();
}