namespace Application.Services.Common.BackgroundJobs;

public interface ICourierJobService
{
    Task CheckExpiredAssignments();
    Task AutoOfflineInactiveCouriers();
}
