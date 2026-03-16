namespace Application.Services.Common.BackgroundJobs;

public interface ICleanupJobService
{
    Task CleanupExpiredResetTokens();
    Task CleanupExpiredRefreshTokens();
}
