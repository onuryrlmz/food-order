using System.Data;
using Application.Services.Common.TokenService;
using Dapper;
using Domain.Dto.Buyer.ScheduledOrder;
using Domain.Entities.Buyer;
using Domain.Service;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Persistence.Contexts;
using Persistence.IRepositories;

namespace Application.Services.Buyer.ScheduledOrderService;

public class ScheduledOrderManager : IScheduledOrderService
{
    private const short StatusScheduled = 1;
    private const short StatusProcessing = 2;
    private const short StatusCancelled = 3;
    private const short StatusConverted = 4;

    private readonly IUnitOfWork _unitOfWork;
    private readonly ITokenAccessor _tokenAccessor;
    private readonly BaseDbContext _context;
    private readonly IConfiguration _configuration;

    public ScheduledOrderManager(IUnitOfWork unitOfWork, ITokenAccessor tokenAccessor, BaseDbContext context, IConfiguration configuration)
    {
        _unitOfWork = unitOfWork;
        _tokenAccessor = tokenAccessor;
        _context = context;
        _configuration = configuration;
    }

    public async Task<ServiceObjectResult<ScheduledOrderDto>> CreateScheduledOrder(CreateScheduledOrderRequestDto requestDto)
    {
        var result = new ServiceObjectResult<ScheduledOrderDto>();
        try
        {
            var token = _tokenAccessor.GetToken();
            if (token == null)
            {
                result.Fail("Unauthorized");
                return result;
            }

            var minHoursAhead = _configuration.GetValue<int>("ScheduledOrder:MinHoursAhead", 1);
            var maxDaysAhead = _configuration.GetValue<int>("ScheduledOrder:MaxDaysAhead", 7);

            // Validate scheduled time is at least N hours in the future
            if (requestDto.ScheduledDeliveryTime < DateTime.UtcNow.AddHours(minHoursAhead))
            {
                result.Fail($"Scheduled delivery time must be at least {minHoursAhead} hour(s) from now");
                return result;
            }

            // Validate scheduled time is no more than N days out
            if (requestDto.ScheduledDeliveryTime > DateTime.UtcNow.AddDays(maxDaysAhead))
            {
                result.Fail($"Scheduled delivery time cannot be more than {maxDaysAhead} days from now");
                return result;
            }

            // Validate restaurant exists and is active
            var restaurant = await _unitOfWork.RestaurantRepository.GetAsync(
                x => x.Id == requestDto.RestaurantId && x.IsActive);
            if (restaurant == null)
            {
                result.Fail("Restaurant not found or not active");
                return result;
            }

            // Calculate ProcessAt (N minutes before scheduled delivery)
            var processMinutesBefore = _configuration.GetValue<int>("ScheduledOrder:ProcessMinutesBefore", 45);
            var processAt = requestDto.ScheduledDeliveryTime.AddMinutes(-processMinutesBefore);

            var scheduledOrder = new ScheduledOrder
            {
                Id = Guid.NewGuid(),
                UserId = token.UserId,
                RestaurantId = requestDto.RestaurantId,
                DeliveryAddressId = requestDto.DeliveryAddressId,
                InvoiceAddressId = requestDto.InvoiceAddressId,
                StatusId = StatusScheduled,
                ScheduledDeliveryTime = requestDto.ScheduledDeliveryTime,
                ProcessAt = processAt,
                PaymentOptionId = requestDto.PaymentOptionId,
                Notes = requestDto.Notes,
                BasketSnapshotJson = requestDto.BasketSnapshotJson,
                CouponCode = requestDto.CouponCode
            };

            await _unitOfWork.ScheduledOrderRepository.AddAsync(scheduledOrder);
            await _unitOfWork.CompleteAsync();

            result.SetData(new ScheduledOrderDto
            {
                Id = scheduledOrder.Id,
                RestaurantId = scheduledOrder.RestaurantId,
                RestaurantName = restaurant.Name,
                StatusId = scheduledOrder.StatusId,
                ScheduledDeliveryTime = scheduledOrder.ScheduledDeliveryTime,
                ProcessAt = scheduledOrder.ProcessAt,
                Notes = scheduledOrder.Notes,
                CreatedDate = scheduledOrder.CreatedDate
            });
        }
        catch (Exception e)
        {
            result.Fail(e);
        }

        return result;
    }

    public async Task<ServiceCollectionResult<ScheduledOrderDto>> GetMyScheduledOrders(int page = 1, int pageSize = 20)
    {
        var result = new ServiceCollectionResult<ScheduledOrderDto>();
        try
        {
            var token = _tokenAccessor.GetToken();
            if (token == null)
            {
                result.Fail("Unauthorized");
                return result;
            }

            pageSize = Math.Min(pageSize, 50);
            var offset = (page - 1) * pageSize;

            const string countQuery = @"
                SELECT COUNT(*) FROM `ScheduledOrder`
                WHERE `UserId` = @userId AND `DeletedDate` IS NULL";

            const string query = @"
                SELECT so.`Id`, so.`RestaurantId`, r.`Name` AS RestaurantName,
                       so.`StatusId`, so.`ScheduledDeliveryTime`, so.`ProcessAt`,
                       so.`Notes`, so.`ConvertedOrderId`, so.`CreatedDate`
                FROM `ScheduledOrder` so
                INNER JOIN `Restaurant` r ON r.`Id` = so.`RestaurantId`
                WHERE so.`UserId` = @userId AND so.`DeletedDate` IS NULL
                ORDER BY so.`ScheduledDeliveryTime` DESC
                LIMIT @pageSize OFFSET @offset";

            var conn = _context.Database.GetDbConnection();
            if (conn.State != ConnectionState.Open)
                await conn.OpenAsync();

            var totalCount = await conn.ExecuteScalarAsync<int>(countQuery, new { userId = token.UserId });
            var orders = (await conn.QueryAsync<ScheduledOrderDto>(query, new { userId = token.UserId, pageSize, offset })).ToList();

            result.SetData(totalCount, orders);
        }
        catch (Exception e)
        {
            result.Fail(e);
        }

        return result;
    }

    public async Task<ServiceObjectResult<ScheduledOrderDetailDto>> GetScheduledOrderDetail(Guid scheduledOrderId)
    {
        var result = new ServiceObjectResult<ScheduledOrderDetailDto>();
        try
        {
            var token = _tokenAccessor.GetToken();
            if (token == null)
            {
                result.Fail("Unauthorized");
                return result;
            }

            const string query = @"
                SELECT so.`Id`, so.`RestaurantId`, r.`Name` AS RestaurantName,
                       so.`StatusId`, so.`ScheduledDeliveryTime`, so.`ProcessAt`,
                       so.`PaymentOptionId`, so.`Notes`, so.`CancellationReason`,
                       so.`ConvertedOrderId`, so.`BasketSnapshotJson`, so.`CouponCode`,
                       so.`CreatedDate`
                FROM `ScheduledOrder` so
                INNER JOIN `Restaurant` r ON r.`Id` = so.`RestaurantId`
                WHERE so.`Id` = @scheduledOrderId AND so.`UserId` = @userId AND so.`DeletedDate` IS NULL";

            var conn = _context.Database.GetDbConnection();
            if (conn.State != ConnectionState.Open)
                await conn.OpenAsync();

            var detail = await conn.QueryFirstOrDefaultAsync<ScheduledOrderDetailDto>(query,
                new { scheduledOrderId, userId = token.UserId });

            if (detail == null)
            {
                result.Fail("Scheduled order not found");
                return result;
            }

            result.SetData(detail);
        }
        catch (Exception e)
        {
            result.Fail(e);
        }

        return result;
    }

    public async Task<ServiceObjectResult<bool>> CancelScheduledOrder(Guid scheduledOrderId, string? reason)
    {
        var result = new ServiceObjectResult<bool>();
        try
        {
            var token = _tokenAccessor.GetToken();
            if (token == null)
            {
                result.Fail("Unauthorized");
                return result;
            }

            var order = await _unitOfWork.ScheduledOrderRepository.GetAsync(
                x => x.Id == scheduledOrderId && x.UserId == token.UserId, enableTracking: true);
            if (order == null)
            {
                result.Fail("Scheduled order not found");
                return result;
            }

            if (order.StatusId != StatusScheduled)
            {
                result.Fail("Only scheduled orders can be cancelled");
                return result;
            }

            order.StatusId = StatusCancelled;
            order.CancellationReason = reason;
            await _unitOfWork.ScheduledOrderRepository.UpdateAsync(order);
            await _unitOfWork.CompleteAsync();

            result.SetData(true);
        }
        catch (Exception e)
        {
            result.Fail(e);
        }

        return result;
    }

    public async Task<ServiceObjectResult<bool>> UpdateScheduledOrder(Guid scheduledOrderId, UpdateScheduledOrderRequestDto requestDto)
    {
        var result = new ServiceObjectResult<bool>();
        try
        {
            var token = _tokenAccessor.GetToken();
            if (token == null)
            {
                result.Fail("Unauthorized");
                return result;
            }

            var order = await _unitOfWork.ScheduledOrderRepository.GetAsync(
                x => x.Id == scheduledOrderId && x.UserId == token.UserId, enableTracking: true);
            if (order == null)
            {
                result.Fail("Scheduled order not found");
                return result;
            }

            if (order.StatusId != StatusScheduled)
            {
                result.Fail("Only scheduled orders can be updated");
                return result;
            }

            if (requestDto.ScheduledDeliveryTime.HasValue)
            {
                var minHoursAhead = _configuration.GetValue<int>("ScheduledOrder:MinHoursAhead", 1);
                if (requestDto.ScheduledDeliveryTime.Value < DateTime.UtcNow.AddHours(minHoursAhead))
                {
                    result.Fail($"Scheduled delivery time must be at least {minHoursAhead} hour(s) from now");
                    return result;
                }

                var processMinutesBefore = _configuration.GetValue<int>("ScheduledOrder:ProcessMinutesBefore", 45);
                order.ScheduledDeliveryTime = requestDto.ScheduledDeliveryTime.Value;
                order.ProcessAt = requestDto.ScheduledDeliveryTime.Value.AddMinutes(-processMinutesBefore);
            }

            if (requestDto.Notes != null)
                order.Notes = requestDto.Notes;

            if (requestDto.DeliveryAddressId.HasValue)
                order.DeliveryAddressId = requestDto.DeliveryAddressId.Value;

            await _unitOfWork.ScheduledOrderRepository.UpdateAsync(order);
            await _unitOfWork.CompleteAsync();

            result.SetData(true);
        }
        catch (Exception e)
        {
            result.Fail(e);
        }

        return result;
    }

    public async Task ProcessDueScheduledOrders()
    {
        try
        {
            const string query = @"
                SELECT `Id` FROM `ScheduledOrder`
                WHERE `StatusId` = @statusScheduled
                  AND `ProcessAt` <= @now
                  AND `DeletedDate` IS NULL";

            var conn = _context.Database.GetDbConnection();
            if (conn.State != ConnectionState.Open)
                await conn.OpenAsync();

            var dueOrderIds = (await conn.QueryAsync<Guid>(query,
                new { statusScheduled = StatusScheduled, now = DateTime.UtcNow })).ToList();

            foreach (var orderId in dueOrderIds)
            {
                try
                {
                    var order = await _unitOfWork.ScheduledOrderRepository.GetAsync(
                        x => x.Id == orderId, enableTracking: true);
                    if (order == null) continue;

                    order.StatusId = StatusProcessing;
                    await _unitOfWork.ScheduledOrderRepository.UpdateAsync(order);
                    await _unitOfWork.CompleteAsync();

                    // TODO: Convert to real order by calling IOrderService.PlaceOrder
                    // This requires an internal method that accepts userId context
                    // For now, mark as converted placeholder
                    // var realOrder = await _orderService.PlaceOrderInternal(order.UserId, ...);
                    // order.ConvertedOrderId = realOrder.Id;
                    // order.StatusId = StatusConverted;
                }
                catch
                {
                    // If conversion fails, cancel the scheduled order
                    var failedOrder = await _unitOfWork.ScheduledOrderRepository.GetAsync(
                        x => x.Id == orderId, enableTracking: true);
                    if (failedOrder != null)
                    {
                        failedOrder.StatusId = StatusCancelled;
                        failedOrder.CancellationReason = "Failed to process scheduled order";
                        await _unitOfWork.ScheduledOrderRepository.UpdateAsync(failedOrder);
                        await _unitOfWork.CompleteAsync();
                    }
                }
            }
        }
        catch (Exception ex)
        {
            // Log error - background job should not throw
            Console.WriteLine($"ProcessDueScheduledOrders error: {ex.Message}");
        }
    }
}
