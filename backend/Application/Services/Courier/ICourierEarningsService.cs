using Domain.Dto.Courier;
using Domain.Service;

namespace Application.Services.Courier;

public interface ICourierEarningsService
{
    Task<ServiceObjectResult<CourierEarningsSummaryDto>> GetEarningsAsync(Guid courierId, DateTime startDate, DateTime endDate);
    Task<ServiceCollectionResult<CourierEarningsHistoryDto>> GetEarningsHistoryAsync(Guid courierId, DateTime startDate, DateTime endDate, int page = 1, int pageSize = 20);
    Task<ServiceObjectResult<bool>> ToggleCourierStatusAsync(Guid courierId, short statusId);
}
