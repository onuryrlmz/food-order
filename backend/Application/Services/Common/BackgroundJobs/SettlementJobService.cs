using Application.Services.Seller.CommissionService;

namespace Application.Services.Common.BackgroundJobs;

public class SettlementJobService : ISettlementJobService
{
    private readonly ICommissionService _commissionService;

    public SettlementJobService(ICommissionService commissionService)
    {
        _commissionService = commissionService;
    }

    public async Task GenerateDailySettlements()
    {
        var yesterday = DateTime.UtcNow.Date.AddDays(-1);
        await _commissionService.GenerateDailySettlements(yesterday);
    }
}
