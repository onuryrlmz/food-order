using Application.Services.Common.NotificationService;
using Domain.Dto.Courier;
using Domain.Entities.Courier;
using Domain.Service;
using Persistence.IRepositories.Courier;

namespace Application.Services.Courier;

public class CourierLocationManager : ICourierLocationService
{
    private readonly ICourierLocationRepository _courierLocationRepository;
    private readonly IRealtimeNotifier _realtimeNotifier;

    public CourierLocationManager(
        ICourierLocationRepository courierLocationRepository,
        IRealtimeNotifier realtimeNotifier)
    {
        _courierLocationRepository = courierLocationRepository;
        _realtimeNotifier = realtimeNotifier;
    }

    public async Task<ServiceObjectResult<bool>> UpdateLocationAsync(Guid courierId, decimal latitude, decimal longitude, Guid? orderId = null)
    {
        var result = new ServiceObjectResult<bool>();
        try
        {
            var location = new CourierLocation
            {
                Id = Guid.NewGuid(),
                CourierId = courierId,
                OrderId = orderId,
                Latitude = latitude,
                Longitude = longitude,
                UpdatedAt = DateTime.UtcNow
            };

            await _courierLocationRepository.AddAsync(location);

            // Push courier location via SignalR to order group
            if (orderId.HasValue)
            {
                _ = _realtimeNotifier.NotifyCourierLocationUpdated(orderId.Value, latitude, longitude);
            }

            result.SetData(true);
            return result;
        }
        catch (Exception ex)
        {
            result.Fail($"Konum güncelleme hatası: {ex.Message}");
            return result;
        }
    }
}
