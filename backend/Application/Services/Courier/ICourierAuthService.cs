using Domain.Dto.Courier;
using Domain.Service;

namespace Application.Services.Courier;

public interface ICourierAuthService
{
    Task<ServiceObjectResult<bool>> RegisterAsync(CourierRegisterRequestDto request);
    Task<ServiceObjectResult<CourierLoginResponseDto>> LoginAsync(CourierLoginRequestDto request);
}
