using System.Data;
using Application.Services.Common.TokenService;
using Base.Enums;
using Dapper;
using Domain.Dto.Seller.Commission;
using Domain.Dto.Seller.Settlement;
using Domain.Entities.Common;
using Domain.Entities.Seller;
using Domain.Service;
using Microsoft.EntityFrameworkCore;
using Persistence.Contexts;
using Persistence.IRepositories;

namespace Application.Services.Seller.CommissionService;

public class CommissionManager : ICommissionService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ITokenAccessor _tokenAccessor;
    private readonly BaseDbContext _context;

    public CommissionManager(IUnitOfWork unitOfWork, ITokenAccessor tokenAccessor, BaseDbContext context)
    {
        _unitOfWork = unitOfWork;
        _tokenAccessor = tokenAccessor;
        _context = context;
    }

    // ===== Platform Commission Schedule =====

    public async Task<ServiceCollectionResult<PlatformCommissionScheduleDto>> GetPlatformSchedules()
    {
        var result = new ServiceCollectionResult<PlatformCommissionScheduleDto>();
        try
        {
            const string query = @"
                SELECT `Id`, `CommissionRate`, `FixedFee`, `EffectiveFrom`, `EffectiveTo`, `Notes`,
                       CASE WHEN `EffectiveFrom` <= UTC_TIMESTAMP() AND (`EffectiveTo` IS NULL OR `EffectiveTo` > UTC_TIMESTAMP()) THEN 1 ELSE 0 END AS IsActive
                FROM `PlatformCommissionSchedule`
                WHERE `DeletedDate` IS NULL
                ORDER BY `EffectiveFrom` DESC";

            var conn = _context.Database.GetDbConnection();
            if (conn.State != ConnectionState.Open) await conn.OpenAsync();

            var schedules = (await conn.QueryAsync<PlatformCommissionScheduleDto>(query)).ToList();
            result.SetData(schedules);
        }
        catch (Exception e) { result.Fail(e); }
        return result;
    }

    public async Task<ServiceObjectResult<PlatformCommissionScheduleDto>> CreatePlatformSchedule(CreatePlatformCommissionDto dto)
    {
        var result = new ServiceObjectResult<PlatformCommissionScheduleDto>();
        try
        {
            var token = _tokenAccessor.GetToken();
            if (token == null) { result.Fail("Unauthorized"); return result; }

            if (dto.CommissionRate < 0 || dto.CommissionRate > 1)
            { result.Fail("Commission rate must be between 0 and 1"); return result; }

            // Close previous open schedule if new one starts before it ends
            var currentActive = await _unitOfWork.PlatformCommissionScheduleRepository.GetAsync(
                x => x.EffectiveFrom <= dto.EffectiveFrom && x.EffectiveTo == null && x.DeletedDate == null,
                enableTracking: true);
            if (currentActive != null)
            {
                currentActive.EffectiveTo = dto.EffectiveFrom;
                await _unitOfWork.PlatformCommissionScheduleRepository.UpdateAsync(currentActive);
            }

            var schedule = new PlatformCommissionSchedule
            {
                Id = Guid.NewGuid(),
                CommissionRate = dto.CommissionRate,
                FixedFee = dto.FixedFee,
                EffectiveFrom = dto.EffectiveFrom,
                SetByUserId = token.UserId,
                Notes = dto.Notes
            };

            await _unitOfWork.PlatformCommissionScheduleRepository.AddAsync(schedule);
            await _unitOfWork.CompleteAsync();

            result.SetData(new PlatformCommissionScheduleDto
            {
                Id = schedule.Id,
                CommissionRate = schedule.CommissionRate,
                FixedFee = schedule.FixedFee,
                EffectiveFrom = schedule.EffectiveFrom,
                Notes = schedule.Notes,
                IsActive = schedule.EffectiveFrom <= DateTime.UtcNow
            });
        }
        catch (Exception e) { result.Fail(e); }
        return result;
    }

    public async Task<ServiceObjectResult<PlatformCommissionScheduleDto>> GetActivePlatformSchedule()
    {
        var result = new ServiceObjectResult<PlatformCommissionScheduleDto>();
        try
        {
            const string query = @"
                SELECT `Id`, `CommissionRate`, `FixedFee`, `EffectiveFrom`, `EffectiveTo`, `Notes`, 1 AS IsActive
                FROM `PlatformCommissionSchedule`
                WHERE `EffectiveFrom` <= UTC_TIMESTAMP() AND (`EffectiveTo` IS NULL OR `EffectiveTo` > UTC_TIMESTAMP())
                  AND `DeletedDate` IS NULL
                ORDER BY `EffectiveFrom` DESC LIMIT 1";

            var conn = _context.Database.GetDbConnection();
            if (conn.State != ConnectionState.Open) await conn.OpenAsync();

            var schedule = await conn.QueryFirstOrDefaultAsync<PlatformCommissionScheduleDto>(query);
            if (schedule == null) { result.Fail("No active platform commission schedule found"); return result; }

            result.SetData(schedule);
        }
        catch (Exception e) { result.Fail(e); }
        return result;
    }

    // ===== Restaurant Commission =====

    public async Task<ServiceObjectResult<RestaurantCommissionDto>> GetRestaurantCommission(Guid restaurantId)
    {
        var result = new ServiceObjectResult<RestaurantCommissionDto>();
        try
        {
            const string query = @"
                SELECT rc.`Id`, rc.`RestaurantId`, r.`Name` AS RestaurantName,
                       rc.`CommissionRate`, rc.`FixedFee`, rc.`EffectiveFrom`, rc.`EffectiveTo`,
                       rc.`Reason`, rc.`Notes`, 1 AS IsActive
                FROM `RestaurantCommission` rc
                INNER JOIN `Restaurant` r ON r.`Id` = rc.`RestaurantId`
                WHERE rc.`RestaurantId` = @restaurantId
                  AND rc.`EffectiveFrom` <= UTC_TIMESTAMP()
                  AND (rc.`EffectiveTo` IS NULL OR rc.`EffectiveTo` > UTC_TIMESTAMP())
                  AND rc.`DeletedDate` IS NULL
                ORDER BY rc.`EffectiveFrom` DESC LIMIT 1";

            var conn = _context.Database.GetDbConnection();
            if (conn.State != ConnectionState.Open) await conn.OpenAsync();

            var commission = await conn.QueryFirstOrDefaultAsync<RestaurantCommissionDto>(query, new { restaurantId });
            if (commission == null) { result.Fail("No custom commission for this restaurant"); return result; }

            result.SetData(commission);
        }
        catch (Exception e) { result.Fail(e); }
        return result;
    }

    public async Task<ServiceCollectionResult<RestaurantCommissionDto>> GetRestaurantCommissionHistory(Guid restaurantId)
    {
        var result = new ServiceCollectionResult<RestaurantCommissionDto>();
        try
        {
            const string query = @"
                SELECT rc.`Id`, rc.`RestaurantId`, r.`Name` AS RestaurantName,
                       rc.`CommissionRate`, rc.`FixedFee`, rc.`EffectiveFrom`, rc.`EffectiveTo`,
                       rc.`Reason`, rc.`Notes`,
                       CASE WHEN rc.`EffectiveFrom` <= UTC_TIMESTAMP() AND (rc.`EffectiveTo` IS NULL OR rc.`EffectiveTo` > UTC_TIMESTAMP()) THEN 1 ELSE 0 END AS IsActive
                FROM `RestaurantCommission` rc
                INNER JOIN `Restaurant` r ON r.`Id` = rc.`RestaurantId`
                WHERE rc.`RestaurantId` = @restaurantId AND rc.`DeletedDate` IS NULL
                ORDER BY rc.`EffectiveFrom` DESC";

            var conn = _context.Database.GetDbConnection();
            if (conn.State != ConnectionState.Open) await conn.OpenAsync();

            var history = (await conn.QueryAsync<RestaurantCommissionDto>(query, new { restaurantId })).ToList();
            result.SetData(history);
        }
        catch (Exception e) { result.Fail(e); }
        return result;
    }

    public async Task<ServiceObjectResult<RestaurantCommissionDto>> SetRestaurantCommission(Guid restaurantId, SetRestaurantCommissionDto dto)
    {
        var result = new ServiceObjectResult<RestaurantCommissionDto>();
        try
        {
            var token = _tokenAccessor.GetToken();
            if (token == null) { result.Fail("Unauthorized"); return result; }

            if (dto.CommissionRate < 0 || dto.CommissionRate > 1)
            { result.Fail("Commission rate must be between 0 and 1"); return result; }

            var restaurant = await _unitOfWork.RestaurantRepository.GetAsync(x => x.Id == restaurantId);
            if (restaurant == null) { result.Fail("Restaurant not found"); return result; }

            var effectiveFrom = dto.EffectiveFrom ?? DateTime.UtcNow;

            // Close current active commission
            var currentActive = await _unitOfWork.RestaurantCommissionRepository.GetAsync(
                x => x.RestaurantId == restaurantId && x.EffectiveTo == null && x.DeletedDate == null,
                enableTracking: true);
            if (currentActive != null)
            {
                currentActive.EffectiveTo = effectiveFrom;
                await _unitOfWork.RestaurantCommissionRepository.UpdateAsync(currentActive);
            }

            var commission = new RestaurantCommission
            {
                Id = Guid.NewGuid(),
                RestaurantId = restaurantId,
                CommissionRate = dto.CommissionRate,
                FixedFee = dto.FixedFee,
                EffectiveFrom = effectiveFrom,
                SetByUserId = token.UserId,
                Reason = dto.Reason,
                Notes = dto.Notes
            };

            await _unitOfWork.RestaurantCommissionRepository.AddAsync(commission);
            await _unitOfWork.CompleteAsync();

            result.SetData(new RestaurantCommissionDto
            {
                Id = commission.Id,
                RestaurantId = restaurantId,
                RestaurantName = restaurant.Name,
                CommissionRate = commission.CommissionRate,
                FixedFee = commission.FixedFee,
                EffectiveFrom = commission.EffectiveFrom,
                Reason = commission.Reason,
                Notes = commission.Notes,
                IsActive = true
            });
        }
        catch (Exception e) { result.Fail(e); }
        return result;
    }

    public async Task<ServiceCollectionResult<RestaurantCommissionDto>> GetMyCommissions()
    {
        var result = new ServiceCollectionResult<RestaurantCommissionDto>();
        try
        {
            var token = _tokenAccessor.GetToken();
            if (token == null) { result.Fail("Unauthorized"); return result; }

            const string query = @"
                SELECT rc.`Id`, rc.`RestaurantId`, r.`Name` AS RestaurantName,
                       rc.`CommissionRate`, rc.`FixedFee`, rc.`EffectiveFrom`, rc.`EffectiveTo`,
                       rc.`Reason`, rc.`Notes`,
                       CASE WHEN rc.`EffectiveFrom` <= UTC_TIMESTAMP() AND (rc.`EffectiveTo` IS NULL OR rc.`EffectiveTo` > UTC_TIMESTAMP()) THEN 1 ELSE 0 END AS IsActive
                FROM `RestaurantCommission` rc
                INNER JOIN `Restaurant` r ON r.`Id` = rc.`RestaurantId`
                INNER JOIN `Seller` s ON s.`Id` = r.`SellerId`
                INNER JOIN `User` u ON u.`SellerId` = s.`Id`
                WHERE u.`Id` = @userId AND rc.`DeletedDate` IS NULL
                  AND rc.`EffectiveTo` IS NULL
                ORDER BY r.`Name`";

            var conn = _context.Database.GetDbConnection();
            if (conn.State != ConnectionState.Open) await conn.OpenAsync();

            var commissions = (await conn.QueryAsync<RestaurantCommissionDto>(query, new { userId = token.UserId })).ToList();
            result.SetData(commissions);
        }
        catch (Exception e) { result.Fail(e); }
        return result;
    }

    // ===== Settlement =====

    public async Task<ServiceCollectionResult<SettlementPeriodDto>> GetSettlementPeriods(short? statusId = null, int page = 1, int pageSize = 20)
    {
        var result = new ServiceCollectionResult<SettlementPeriodDto>();
        try
        {
            pageSize = Math.Min(pageSize, 50);
            var offset = (page - 1) * pageSize;

            var where = "sp.`DeletedDate` IS NULL";
            if (statusId.HasValue) where += " AND sp.`StatusId` = @statusId";

            var countQuery = $"SELECT COUNT(*) FROM `SettlementPeriod` sp WHERE {where}";
            var query = $@"
                SELECT sp.`Id`, sp.`SellerId`, sp.`RestaurantId`, r.`Name` AS RestaurantName,
                       sp.`PeriodDate`, sp.`TotalOrderCount`, sp.`TotalOrderAmount`,
                       sp.`TotalCommission`, sp.`TotalFixedFee`, sp.`TotalNetAmount`,
                       sp.`StatusId`, sp.`IBAN`, sp.`BankTransferRef`,
                       sp.`ApprovedAt`, sp.`PaidAt`, sp.`Notes`
                FROM `SettlementPeriod` sp
                INNER JOIN `Restaurant` r ON r.`Id` = sp.`RestaurantId`
                WHERE {where}
                ORDER BY sp.`PeriodDate` DESC
                LIMIT @pageSize OFFSET @offset";

            var conn = _context.Database.GetDbConnection();
            if (conn.State != ConnectionState.Open) await conn.OpenAsync();

            var totalCount = await conn.ExecuteScalarAsync<int>(countQuery, new { statusId });
            var periods = (await conn.QueryAsync<SettlementPeriodDto>(query, new { statusId, pageSize, offset })).ToList();

            result.SetData(totalCount, periods);
        }
        catch (Exception e) { result.Fail(e); }
        return result;
    }

    public async Task<ServiceObjectResult<SettlementPeriodDetailDto>> GetSettlementPeriodDetail(Guid periodId)
    {
        var result = new ServiceObjectResult<SettlementPeriodDetailDto>();
        try
        {
            var conn = _context.Database.GetDbConnection();
            if (conn.State != ConnectionState.Open) await conn.OpenAsync();

            const string periodQuery = @"
                SELECT sp.`Id`, sp.`SellerId`, sp.`RestaurantId`, r.`Name` AS RestaurantName,
                       sp.`PeriodDate`, sp.`TotalOrderCount`, sp.`TotalOrderAmount`,
                       sp.`TotalCommission`, sp.`TotalFixedFee`, sp.`TotalNetAmount`,
                       sp.`StatusId`, sp.`IBAN`, sp.`BankTransferRef`,
                       sp.`ApprovedAt`, sp.`PaidAt`, sp.`Notes`
                FROM `SettlementPeriod` sp
                INNER JOIN `Restaurant` r ON r.`Id` = sp.`RestaurantId`
                WHERE sp.`Id` = @periodId AND sp.`DeletedDate` IS NULL";

            var period = await conn.QueryFirstOrDefaultAsync<SettlementPeriodDetailDto>(periodQuery, new { periodId });
            if (period == null) { result.Fail("Settlement period not found"); return result; }

            const string itemsQuery = @"
                SELECT `Id`, `OrderId`, `OrderAmount`, `CommissionRate`, `CommissionAmount`,
                       `FixedFee`, `NetAmount`, `CommissionSourceType`, `CreatedDate`
                FROM `SettlementItem`
                WHERE `SettlementPeriodId` = @periodId AND `DeletedDate` IS NULL
                ORDER BY `CreatedDate` ASC";

            period.Items = (await conn.QueryAsync<SettlementItemDto>(itemsQuery, new { periodId })).ToList();
            result.SetData(period);
        }
        catch (Exception e) { result.Fail(e); }
        return result;
    }

    public async Task<ServiceCollectionResult<SettlementPeriodDto>> GetMySettlementPeriods(int page = 1, int pageSize = 20)
    {
        var result = new ServiceCollectionResult<SettlementPeriodDto>();
        try
        {
            var token = _tokenAccessor.GetToken();
            if (token == null) { result.Fail("Unauthorized"); return result; }

            pageSize = Math.Min(pageSize, 50);
            var offset = (page - 1) * pageSize;

            const string countQuery = @"
                SELECT COUNT(*) FROM `SettlementPeriod` sp
                INNER JOIN `Restaurant` r ON r.`Id` = sp.`RestaurantId`
                INNER JOIN `Seller` s ON s.`Id` = r.`SellerId`
                INNER JOIN `User` u ON u.`SellerId` = s.`Id`
                WHERE u.`Id` = @userId AND sp.`DeletedDate` IS NULL";

            const string query = @"
                SELECT sp.`Id`, sp.`SellerId`, sp.`RestaurantId`, r.`Name` AS RestaurantName,
                       sp.`PeriodDate`, sp.`TotalOrderCount`, sp.`TotalOrderAmount`,
                       sp.`TotalCommission`, sp.`TotalFixedFee`, sp.`TotalNetAmount`,
                       sp.`StatusId`, sp.`IBAN`, sp.`BankTransferRef`,
                       sp.`ApprovedAt`, sp.`PaidAt`, sp.`Notes`
                FROM `SettlementPeriod` sp
                INNER JOIN `Restaurant` r ON r.`Id` = sp.`RestaurantId`
                INNER JOIN `Seller` s ON s.`Id` = r.`SellerId`
                INNER JOIN `User` u ON u.`SellerId` = s.`Id`
                WHERE u.`Id` = @userId AND sp.`DeletedDate` IS NULL
                ORDER BY sp.`PeriodDate` DESC
                LIMIT @pageSize OFFSET @offset";

            var conn = _context.Database.GetDbConnection();
            if (conn.State != ConnectionState.Open) await conn.OpenAsync();

            var totalCount = await conn.ExecuteScalarAsync<int>(countQuery, new { userId = token.UserId });
            var periods = (await conn.QueryAsync<SettlementPeriodDto>(query, new { userId = token.UserId, pageSize, offset })).ToList();

            result.SetData(totalCount, periods);
        }
        catch (Exception e) { result.Fail(e); }
        return result;
    }

    public async Task<ServiceObjectResult<SettlementPeriodDetailDto>> GetMySettlementPeriodDetail(Guid periodId)
    {
        var result = new ServiceObjectResult<SettlementPeriodDetailDto>();
        try
        {
            var token = _tokenAccessor.GetToken();
            if (token == null) { result.Fail("Unauthorized"); return result; }

            // Verify ownership
            var conn = _context.Database.GetDbConnection();
            if (conn.State != ConnectionState.Open) await conn.OpenAsync();

            const string ownerCheck = @"
                SELECT COUNT(*) FROM `SettlementPeriod` sp
                INNER JOIN `Restaurant` r ON r.`Id` = sp.`RestaurantId`
                INNER JOIN `Seller` s ON s.`Id` = r.`SellerId`
                INNER JOIN `User` u ON u.`SellerId` = s.`Id`
                WHERE sp.`Id` = @periodId AND u.`Id` = @userId";

            var count = await conn.ExecuteScalarAsync<int>(ownerCheck, new { periodId, userId = token.UserId });
            if (count == 0) { result.Fail("Settlement period not found"); return result; }

            return await GetSettlementPeriodDetail(periodId);
        }
        catch (Exception e) { result.Fail(e); }
        return result;
    }

    public async Task<ServiceObjectResult<bool>> ApproveSettlement(Guid periodId)
    {
        var result = new ServiceObjectResult<bool>();
        try
        {
            var token = _tokenAccessor.GetToken();
            var period = await _unitOfWork.SettlementPeriodRepository.GetAsync(x => x.Id == periodId, enableTracking: true);
            if (period == null) { result.Fail("Settlement period not found"); return result; }
            if (period.StatusId != (short)SettlementStatusEnums.Pending) { result.Fail("Only pending settlements can be approved"); return result; }

            period.StatusId = (short)SettlementStatusEnums.Approved;
            period.ApprovedAt = DateTime.UtcNow;
            period.ApprovedByUserId = token?.UserId;
            await _unitOfWork.SettlementPeriodRepository.UpdateAsync(period);
            await _unitOfWork.CompleteAsync();

            result.SetData(true);
        }
        catch (Exception e) { result.Fail(e); }
        return result;
    }

    public async Task<ServiceObjectResult<bool>> PaySettlement(Guid periodId, PaySettlementDto dto)
    {
        var result = new ServiceObjectResult<bool>();
        try
        {
            var period = await _unitOfWork.SettlementPeriodRepository.GetAsync(x => x.Id == periodId, enableTracking: true);
            if (period == null) { result.Fail("Settlement period not found"); return result; }
            if (period.StatusId != (short)SettlementStatusEnums.Approved) { result.Fail("Only approved settlements can be marked as paid"); return result; }

            period.StatusId = (short)SettlementStatusEnums.Paid;
            period.PaidAt = DateTime.UtcNow;
            period.BankTransferRef = dto.BankTransferRef;
            if (!string.IsNullOrEmpty(dto.Notes)) period.Notes = dto.Notes;
            await _unitOfWork.SettlementPeriodRepository.UpdateAsync(period);
            await _unitOfWork.CompleteAsync();

            result.SetData(true);
        }
        catch (Exception e) { result.Fail(e); }
        return result;
    }

    public async Task<ServiceObjectResult<bool>> CancelSettlement(Guid periodId, string? reason)
    {
        var result = new ServiceObjectResult<bool>();
        try
        {
            var period = await _unitOfWork.SettlementPeriodRepository.GetAsync(x => x.Id == periodId, enableTracking: true);
            if (period == null) { result.Fail("Settlement period not found"); return result; }
            if (period.StatusId == (short)SettlementStatusEnums.Paid) { result.Fail("Paid settlements cannot be cancelled"); return result; }

            period.StatusId = (short)SettlementStatusEnums.Cancelled;
            period.Notes = reason;
            await _unitOfWork.SettlementPeriodRepository.UpdateAsync(period);
            await _unitOfWork.CompleteAsync();

            result.SetData(true);
        }
        catch (Exception e) { result.Fail(e); }
        return result;
    }

    // ===== Commission Resolution =====

    public async Task<(decimal commissionRate, decimal fixedFee, short sourceType, Guid sourceId)> ResolveCommission(Guid restaurantId)
    {
        var conn = _context.Database.GetDbConnection();
        if (conn.State != ConnectionState.Open) await conn.OpenAsync();

        // 1. Check restaurant-specific commission
        const string restaurantQuery = @"
            SELECT `Id`, `CommissionRate`, `FixedFee`
            FROM `RestaurantCommission`
            WHERE `RestaurantId` = @restaurantId
              AND `EffectiveFrom` <= UTC_TIMESTAMP()
              AND (`EffectiveTo` IS NULL OR `EffectiveTo` > UTC_TIMESTAMP())
              AND `DeletedDate` IS NULL
            ORDER BY `EffectiveFrom` DESC LIMIT 1";

        var restaurantComm = await conn.QueryFirstOrDefaultAsync<dynamic>(restaurantQuery, new { restaurantId });
        if (restaurantComm != null)
        {
            return ((decimal)restaurantComm.CommissionRate, (decimal)restaurantComm.FixedFee,
                    (short)CommissionSourceTypeEnums.RestaurantCustom, (Guid)restaurantComm.Id);
        }

        // 2. Fall back to platform default
        const string platformQuery = @"
            SELECT `Id`, `CommissionRate`, `FixedFee`
            FROM `PlatformCommissionSchedule`
            WHERE `EffectiveFrom` <= UTC_TIMESTAMP()
              AND (`EffectiveTo` IS NULL OR `EffectiveTo` > UTC_TIMESTAMP())
              AND `DeletedDate` IS NULL
            ORDER BY `EffectiveFrom` DESC LIMIT 1";

        var platformComm = await conn.QueryFirstOrDefaultAsync<dynamic>(platformQuery);
        if (platformComm != null)
        {
            return ((decimal)platformComm.CommissionRate, (decimal)platformComm.FixedFee,
                    (short)CommissionSourceTypeEnums.Platform, (Guid)platformComm.Id);
        }

        // 3. Absolute fallback
        return (0.10m, 5.00m, (short)CommissionSourceTypeEnums.Platform, Guid.Empty);
    }

    // ===== Daily Settlement Generation =====

    public async Task GenerateDailySettlements(DateTime periodDate)
    {
        try
        {
            var conn = _context.Database.GetDbConnection();
            if (conn.State != ConnectionState.Open) await conn.OpenAsync();

            var dateOnly = periodDate.Date;

            // Group unassigned settlement items by seller+restaurant+date
            const string query = @"
                SELECT `SellerId`, `RestaurantId`, `PeriodDate`,
                       COUNT(*) AS TotalOrderCount,
                       SUM(`OrderAmount`) AS TotalOrderAmount,
                       SUM(`CommissionAmount`) AS TotalCommission,
                       SUM(`FixedFee`) AS TotalFixedFee,
                       SUM(`NetAmount`) AS TotalNetAmount
                FROM `SettlementItem`
                WHERE `PeriodDate` = @dateOnly
                  AND `SettlementPeriodId` IS NULL
                  AND `DeletedDate` IS NULL
                GROUP BY `SellerId`, `RestaurantId`, `PeriodDate`";

            var groups = await conn.QueryAsync<dynamic>(query, new { dateOnly });

            foreach (var group in groups)
            {
                // Get seller IBAN
                const string ibanQuery = "SELECT `IBAN` FROM `Seller` WHERE `Id` = @sellerId";
                var iban = await conn.ExecuteScalarAsync<string>(ibanQuery, new { sellerId = (Guid)group.SellerId });

                var period = new SettlementPeriod
                {
                    Id = Guid.NewGuid(),
                    SellerId = (Guid)group.SellerId,
                    RestaurantId = (Guid)group.RestaurantId,
                    PeriodDate = dateOnly,
                    TotalOrderCount = (int)group.TotalOrderCount,
                    TotalOrderAmount = (decimal)group.TotalOrderAmount,
                    TotalCommission = (decimal)group.TotalCommission,
                    TotalFixedFee = (decimal)group.TotalFixedFee,
                    TotalNetAmount = (decimal)group.TotalNetAmount,
                    StatusId = (short)SettlementStatusEnums.Pending,
                    IBAN = iban
                };

                await _unitOfWork.SettlementPeriodRepository.AddAsync(period);
                await _unitOfWork.CompleteAsync();

                // Assign items to this period
                const string updateQuery = @"
                    UPDATE `SettlementItem`
                    SET `SettlementPeriodId` = @periodId, `UpdatedDate` = UTC_TIMESTAMP()
                    WHERE `SellerId` = @sellerId AND `RestaurantId` = @restaurantId
                      AND `PeriodDate` = @dateOnly AND `SettlementPeriodId` IS NULL
                      AND `DeletedDate` IS NULL";

                await conn.ExecuteAsync(updateQuery, new
                {
                    periodId = period.Id,
                    sellerId = (Guid)group.SellerId,
                    restaurantId = (Guid)group.RestaurantId,
                    dateOnly
                });
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"GenerateDailySettlements error: {ex.Message}");
        }
    }
}
