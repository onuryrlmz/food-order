using Base.Enums;
using Domain.Dto.Courier;
using Domain.Entities.Buyer;
using Domain.Entities.Common;
using Domain.Service;
using Microsoft.EntityFrameworkCore;
using Persistence.Contexts;
using Persistence.IRepositories;

namespace Application.Services.Courier;

public class CourierOrderManager : ICourierOrderService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly BaseDbContext _context;

    public CourierOrderManager(IUnitOfWork unitOfWork, BaseDbContext context)
    {
        _unitOfWork = unitOfWork;
        _context = context;
    }

    public async Task<ServiceCollectionResult<CourierOrderDto>> GetActiveOrdersAsync(Guid courierId)
    {
        var result = new ServiceCollectionResult<CourierOrderDto>();
        try
        {
            var activeStatuses = new List<short>
            {
                (short)AuthorizationServiceEnums.OrderStatusEnums.Preparing,
                (short)AuthorizationServiceEnums.OrderStatusEnums.OnTheWay
            };

            var orders = await _unitOfWork.OrderRepository.GetListAsync(
                o => o.CourierId == courierId && activeStatuses.Contains(o.StatusId),
                orderBy: q => q.OrderByDescending(o => o.CreatedDate));

            var restaurantIds = orders.Items.Select(o => o.RestaurantId).Distinct().ToList();
            var addressIds = orders.Items.Select(o => o.DeliveryAddressId).Distinct().ToList();

            var restaurants = await _context.Set<Domain.Entities.Seller.Restaurant>()
                .Where(r => restaurantIds.Contains(r.Id))
                .ToDictionaryAsync(r => r.Id, r => r.Name);

            var addresses = await _context.Set<Address>()
                .Where(a => addressIds.Contains(a.Id))
                .ToDictionaryAsync(a => a.Id, a => a.AddressLine1);

            var dtos = orders.Items.Select(o =>
            {
                var deliveryArea = addresses.GetValueOrDefault(o.DeliveryAddressId, "Bilinmiyor");

                return new CourierOrderDto
                {
                    OrderId = o.Id,
                    RestaurantName = restaurants.GetValueOrDefault(o.RestaurantId, "Bilinmiyor"),
                    DeliveryArea = deliveryArea,
                    DeliveryDistanceKm = o.DeliveryDistanceKm,
                    StatusId = o.StatusId,
                    StatusName = ((AuthorizationServiceEnums.OrderStatusEnums)o.StatusId).ToString(),
                    CreatedDate = o.CreatedDate
                };
            }).ToList();

            result.SetData(dtos);
            return result;
        }
        catch (Exception ex)
        {
            result.Fail($"Aktif siparişleri getirme hatası: {ex.Message}");
            return result;
        }
    }

    public async Task<ServiceCollectionResult<CourierOrderDto>> GetOrderHistoryAsync(Guid courierId, int month, int year)
    {
        var result = new ServiceCollectionResult<CourierOrderDto>();
        try
        {
            var startDate = new DateTime(year, month, 1, 0, 0, 0, DateTimeKind.Utc);
            var endDate = startDate.AddMonths(1).AddTicks(-1);

            var completedStatus = (short)AuthorizationServiceEnums.OrderStatusEnums.Delivered;

            var orders = await _unitOfWork.OrderRepository.GetListAsync(
                o => o.CourierId == courierId
                    && o.StatusId == completedStatus
                    && o.UpdatedDate >= startDate
                    && o.UpdatedDate <= endDate,
                orderBy: q => q.OrderByDescending(o => o.CreatedDate));

            var restaurantIds = orders.Items.Select(o => o.RestaurantId).Distinct().ToList();
            var restaurants = await _context.Set<Domain.Entities.Seller.Restaurant>()
                .Where(r => restaurantIds.Contains(r.Id))
                .ToDictionaryAsync(r => r.Id, r => r.Name);

            var dtos = orders.Items.Select(o => new CourierOrderDto
            {
                OrderId = o.Id,
                RestaurantName = restaurants.GetValueOrDefault(o.RestaurantId, "Bilinmiyor"),
                DeliveryArea = "Bölge",
                DeliveryDistanceKm = o.DeliveryDistanceKm,
                StatusId = o.StatusId,
                StatusName = ((AuthorizationServiceEnums.OrderStatusEnums)o.StatusId).ToString(),
                CreatedDate = o.CreatedDate
            }).ToList();

            result.SetData(dtos);
            return result;
        }
        catch (Exception ex)
        {
            result.Fail($"Sipariş geçmişini getirme hatası: {ex.Message}");
            return result;
        }
    }

    public async Task<ServiceObjectResult<bool>> DeliverOrderAsync(Guid courierId, Guid orderId)
    {
        var result = new ServiceObjectResult<bool>();
        try
        {
            var order = await _unitOfWork.OrderRepository.GetAsync(o => o.Id == orderId, enableTracking: true);
            if (order == null)
            {
                result.Fail("Sipariş bulunamadı.");
                return result;
            }

            // Validate courier ownership
            if (order.CourierId != courierId && order.PickedUpByCourierId != courierId)
            {
                result.Fail("Bu siparişi teslim etme yetkiniz yok.");
                return result;
            }

            if (order.StatusId != (short)AuthorizationServiceEnums.OrderStatusEnums.OnTheWay)
            {
                result.Fail("Sipariş 'Yolda' durumunda değil.");
                return result;
            }

            order.StatusId = (short)AuthorizationServiceEnums.OrderStatusEnums.Delivered;
            order.DeliveredAt = DateTime.UtcNow;
            _unitOfWork.OrderRepository.Update(order);
            await _unitOfWork.CompleteAsync();

            result.SetData(true);
            result.AddSuccessMessage("Sipariş teslim edildi.");
        }
        catch (Exception ex)
        {
            result.Fail($"Hata: {ex.Message}");
        }
        return result;
    }
}
