namespace Application.Services.Common.BackgroundJobs;

public interface ISettlementJobService
{
    Task GenerateDailySettlements();
}
