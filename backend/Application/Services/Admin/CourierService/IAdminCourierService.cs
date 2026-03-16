using Domain.Dto.Admin.Courier;
using Domain.Service;

namespace Application.Services.Admin.CourierService;

public interface IAdminCourierService
{
    Task<ServiceCollectionResult<AdminCourierListDto>> GetCouriersAsync(int page, int pageSize);
    Task<ServiceObjectResult<bool>> ApproveCourierAsync(ApproveCourierDto request);
}
