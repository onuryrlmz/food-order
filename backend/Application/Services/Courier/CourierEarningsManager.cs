using System.Data;
using Base.Enums;
using Dapper;
using Domain.Dto.Courier;
using Domain.Service;
using Microsoft.EntityFrameworkCore;
using Persistence.Contexts;
using Persistence.IRepositories;
using Persistence.IRepositories.Common;

namespace Application.Services.Courier;

public class CourierEarningsManager : ICourierEarningsService
{
    private readonly BaseDbContext _context;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IUserRepository _userRepository;

    public CourierEarningsManager(BaseDbContext context, IUnitOfWork unitOfWork, IUserRepository userRepository)
    {
        _context = context;
        _unitOfWork = unitOfWork;
        _userRepository = userRepository;
    }

    public async Task<ServiceObjectResult<CourierEarningsSummaryDto>> GetEarningsAsync(Guid courierId, DateTime startDate, DateTime endDate)
    {
        var result = new ServiceObjectResult<CourierEarningsSummaryDto>();
        try
        {
            const string query = @"
                SELECT
                    COALESCE(SUM(o.ShipmentPrice), 0) AS TotalEarnings,
                    COUNT(*) AS TotalDeliveries,
                    COALESCE(AVG(o.ShipmentPrice), 0) AS AveragePerDelivery
                FROM `Order` o
                WHERE (o.CourierId = @CourierId OR o.PickedUpByCourierId = @CourierId)
                    AND o.StatusId = 8
                    AND o.DeliveredAt >= @StartDate
                    AND o.DeliveredAt <= @EndDate
                    AND o.DeletedDate IS NULL
            ";

            var conn = _context.Database.GetDbConnection();
            if (conn.State != ConnectionState.Open)
                await conn.OpenAsync();

            var summary = await conn.QueryFirstOrDefaultAsync<CourierEarningsSummaryDto>(
                query,
                new { CourierId = courierId.ToString(), StartDate = startDate, EndDate = endDate });

            result.SetData(summary ?? new CourierEarningsSummaryDto());
        }
        catch (Exception e)
        {
            result.Fail(e);
        }
        return result;
    }

    public async Task<ServiceCollectionResult<CourierEarningsHistoryDto>> GetEarningsHistoryAsync(Guid courierId, DateTime startDate, DateTime endDate, int page = 1, int pageSize = 20)
    {
        var result = new ServiceCollectionResult<CourierEarningsHistoryDto>();
        try
        {
            const string query = @"
                SELECT
                    o.Id AS OrderId,
                    r.Name AS RestaurantName,
                    o.ShipmentPrice,
                    o.DeliveredAt
                FROM `Order` o
                INNER JOIN `Restaurant` r ON r.Id = o.RestaurantId
                WHERE (o.CourierId = @CourierId OR o.PickedUpByCourierId = @CourierId)
                    AND o.StatusId = 8
                    AND o.DeliveredAt >= @StartDate
                    AND o.DeliveredAt <= @EndDate
                    AND o.DeletedDate IS NULL
                ORDER BY o.DeliveredAt DESC
                LIMIT @PageSize OFFSET @Offset
            ";

            const string countQuery = @"
                SELECT COUNT(*)
                FROM `Order` o
                WHERE (o.CourierId = @CourierId OR o.PickedUpByCourierId = @CourierId)
                    AND o.StatusId = 8
                    AND o.DeliveredAt >= @StartDate
                    AND o.DeliveredAt <= @EndDate
                    AND o.DeletedDate IS NULL
            ";

            var conn = _context.Database.GetDbConnection();
            if (conn.State != ConnectionState.Open)
                await conn.OpenAsync();

            var offset = (page - 1) * pageSize;
            var items = (await conn.QueryAsync<CourierEarningsHistoryDto>(
                query,
                new { CourierId = courierId.ToString(), StartDate = startDate, EndDate = endDate, PageSize = pageSize, Offset = offset })).ToList();

            var totalCount = await conn.ExecuteScalarAsync<int>(
                countQuery,
                new { CourierId = courierId.ToString(), StartDate = startDate, EndDate = endDate });

            result.SetData(totalCount, items);
        }
        catch (Exception e)
        {
            result.Fail(e);
        }
        return result;
    }

    public async Task<ServiceObjectResult<bool>> ToggleCourierStatusAsync(Guid courierId, short statusId)
    {
        var result = new ServiceObjectResult<bool>();
        try
        {
            // Validate status
            if (!Enum.IsDefined(typeof(AuthorizationServiceEnums.CourierStatusEnums), statusId))
            {
                result.Fail("Geçersiz kurye durumu.");
                return result;
            }

            var user = await _userRepository.GetAsync(u => u.Id == courierId, enableTracking: true);
            if (user == null)
            {
                result.Fail("Kurye bulunamadı.");
                return result;
            }

            if (user.UserRoleId != (short)AuthorizationServiceEnums.UserRoleEnums.Courier
                && user.UserRoleId != (short)AuthorizationServiceEnums.UserRoleEnums.CourierCompanyAdmin)
            {
                result.Fail("Bu kullanıcı kurye değil.");
                return result;
            }

            // If trying to go Online, check no active deliveries prevent it
            // If trying to go Offline, check no active deliveries
            if (statusId == (short)AuthorizationServiceEnums.CourierStatusEnums.Offline)
            {
                var activeOrders = await _unitOfWork.OrderRepository.GetListAsync(
                    o => (o.CourierId == courierId || o.PickedUpByCourierId == courierId)
                        && (o.StatusId == (short)AuthorizationServiceEnums.OrderStatusEnums.Preparing
                            || o.StatusId == (short)AuthorizationServiceEnums.OrderStatusEnums.OnTheWay));

                if (activeOrders.Count > 0)
                {
                    result.Fail("Aktif teslimatlarınız varken çevrimdışı olamazsınız.");
                    return result;
                }
            }

            user.CourierStatusId = statusId;
            await _userRepository.UpdateAsync(user);

            result.SetData(true);
        }
        catch (Exception e)
        {
            result.Fail(e);
        }
        return result;
    }
}
