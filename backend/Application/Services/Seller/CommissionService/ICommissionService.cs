using Domain.Dto.Seller.Commission;
using Domain.Dto.Seller.Settlement;
using Domain.Service;

namespace Application.Services.Seller.CommissionService;

public interface ICommissionService
{
    // Platform Commission Schedule
    Task<ServiceCollectionResult<PlatformCommissionScheduleDto>> GetPlatformSchedules();
    Task<ServiceObjectResult<PlatformCommissionScheduleDto>> CreatePlatformSchedule(CreatePlatformCommissionDto dto);
    Task<ServiceObjectResult<PlatformCommissionScheduleDto>> GetActivePlatformSchedule();

    // Restaurant Commission
    Task<ServiceObjectResult<RestaurantCommissionDto>> GetRestaurantCommission(Guid restaurantId);
    Task<ServiceCollectionResult<RestaurantCommissionDto>> GetRestaurantCommissionHistory(Guid restaurantId);
    Task<ServiceObjectResult<RestaurantCommissionDto>> SetRestaurantCommission(Guid restaurantId, SetRestaurantCommissionDto dto);
    Task<ServiceCollectionResult<RestaurantCommissionDto>> GetMyCommissions();

    // Settlement
    Task<ServiceCollectionResult<SettlementPeriodDto>> GetSettlementPeriods(short? statusId = null, int page = 1, int pageSize = 20);
    Task<ServiceObjectResult<SettlementPeriodDetailDto>> GetSettlementPeriodDetail(Guid periodId);
    Task<ServiceCollectionResult<SettlementPeriodDto>> GetMySettlementPeriods(int page = 1, int pageSize = 20);
    Task<ServiceObjectResult<SettlementPeriodDetailDto>> GetMySettlementPeriodDetail(Guid periodId);
    Task<ServiceObjectResult<bool>> ApproveSettlement(Guid periodId);
    Task<ServiceObjectResult<bool>> PaySettlement(Guid periodId, PaySettlementDto dto);
    Task<ServiceObjectResult<bool>> CancelSettlement(Guid periodId, string? reason);

    // Commission resolution (used by OrderManager/PaymentManager)
    Task<(decimal commissionRate, decimal fixedFee, short sourceType, Guid sourceId)> ResolveCommission(Guid restaurantId);

    // Settlement item creation — called when an order is delivered.
    // Idempotent and does NOT commit; the caller commits within its own transaction.
    Task CreateSettlementForDeliveredOrder(Guid orderId);

    // Daily settlement job
    Task GenerateDailySettlements(DateTime periodDate);
}
