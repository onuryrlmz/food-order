using Domain.Dto.Courier;
using Domain.Entities.Courier;
using Domain.Service;
using Persistence.IRepositories.Courier;

namespace Application.Services.Courier;

public class CourierLocationManager : ICourierLocationService
{
    private readonly ICourierLocationRepository _courierLocationRepository;

    public CourierLocationManager(ICourierLocationRepository courierLocationRepository)
    {
        _courierLocationRepository = courierLocationRepository;
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

            // Rate limiting için eski kayıtları temizle (son 24 saatten eskiler)
            // Bu işlem bir background job ile yapılabilir

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
