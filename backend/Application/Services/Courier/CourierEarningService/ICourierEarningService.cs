using Domain.Dto.Courier;
using Domain.Service;

namespace Application.Services.Courier.CourierEarningService;

public interface ICourierEarningService
{
    Task<ServiceCollectionResult> GetMyEarnings(DateTime? from = null, DateTime? to = null, int page = 1, int pageSize = 20);
    Task<ServiceObjectResult<EarningSummaryResponseDto>> GetEarningSummary();
    Task<ServiceObjectResult<bool>> SettleEarnings(List<Guid> earningIds);
    Task<ServiceObjectResult<bool>> CreateEarning(Guid deliveryAssignmentId);
}
