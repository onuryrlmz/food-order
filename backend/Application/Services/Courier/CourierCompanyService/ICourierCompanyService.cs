using Domain.Dto.Courier;
using Domain.Dto.Admin.Courier;
using Domain.Service;

namespace Application.Services.Courier.CourierCompanyService;

public interface ICourierCompanyService
{
    // Company admin self-service
    Task<ServiceObjectResult<CourierCompanyResponseDto>> Register(RegisterCourierCompanyRequestDto requestDto);
    Task<ServiceObjectResult<CourierCompanyResponseDto>> GetProfile();
    Task<ServiceObjectResult<bool>> UpdateProfile(RegisterCourierCompanyRequestDto requestDto);
    Task<ServiceCollectionResult> GetMembers(int page = 1, int pageSize = 20);
    Task<ServiceObjectResult<bool>> AddMember(Guid courierId);
    Task<ServiceObjectResult<bool>> RemoveMember(Guid courierId);
    Task<ServiceCollectionResult> GetCompanyEarnings(DateTime? from = null, DateTime? to = null);

    // Admin
    Task<ServiceCollectionResult> GetAllCompaniesForAdmin(int page = 1, int pageSize = 20, short? statusId = null);
    Task<ServiceObjectResult<bool>> ApproveCompany(Guid companyId);
    Task<ServiceObjectResult<bool>> SuspendCompany(Guid companyId);
}
