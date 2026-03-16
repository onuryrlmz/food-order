using Domain.Dto.Courier.Company;
using Domain.Service;

namespace Application.Services.Courier.CourierCompanyService;

public interface ICourierCompanyService
{
    // Company management (CourierCompanyAdmin)
    Task<ServiceObjectResult<CourierCompanyDto>> RegisterCompanyAsync(Guid ownerUserId, RegisterCourierCompanyRequestDto request);
    Task<ServiceObjectResult<CourierCompanyDto>> GetMyCompanyAsync(Guid ownerUserId);
    Task<ServiceObjectResult<bool>> UpdateCompanyAsync(Guid ownerUserId, UpdateCourierCompanyRequestDto request);

    // Member management (CourierCompanyAdmin)
    Task<ServiceCollectionResult<CourierCompanyMemberDto>> SearchCouriersAsync(string query);
    Task<ServiceObjectResult<bool>> RequestCourierMembershipAsync(Guid ownerUserId, Guid courierId);
    Task<ServiceCollectionResult<CourierCompanyMemberDto>> GetCompanyMembersAsync(Guid ownerUserId);
    Task<ServiceObjectResult<bool>> RemoveMemberAsync(Guid ownerUserId, Guid memberId);

    // Courier side
    Task<ServiceCollectionResult<CompanyInviteDto>> GetCompanyInvitesAsync(Guid courierId);
    Task<ServiceObjectResult<bool>> AcceptCompanyInviteAsync(Guid courierId, Guid inviteId);
    Task<ServiceObjectResult<bool>> RejectCompanyInviteAsync(Guid courierId, Guid inviteId);
    Task<ServiceObjectResult<bool>> LeaveCompanyAsync(Guid courierId);

    // Restaurant <-> Company (from seller side)
    Task<ServiceObjectResult<bool>> InviteCompanyToRestaurantAsync(Guid restaurantId, Guid companyId);
    Task<ServiceCollectionResult<RestaurantCourierCompanyDto>> GetRestaurantCompaniesAsync(Guid restaurantId);
    Task<ServiceObjectResult<bool>> RemoveCompanyFromRestaurantAsync(Guid restaurantId, Guid companyId);

    // Restaurant invites (CourierCompanyAdmin side)
    Task<ServiceCollectionResult<RestaurantCourierCompanyDto>> GetRestaurantInvitesAsync(Guid ownerUserId);
    Task<ServiceObjectResult<bool>> AcceptRestaurantInviteAsync(Guid ownerUserId, Guid inviteId);
    Task<ServiceObjectResult<bool>> RejectRestaurantInviteAsync(Guid ownerUserId, Guid inviteId);

    // Pickup flow
    Task<ServiceCollectionResult<PickupOrderDto>> GetPendingPickupOrdersAsync(Guid courierId, Guid restaurantId, int page, int size);
    Task<ServiceObjectResult<bool>> ConfirmPickupAsync(Guid courierId, Guid orderId);

    // Admin
    Task<ServiceCollectionResult<CourierCompanyDto>> GetAllCompaniesAsync(int page, int size);
    Task<ServiceObjectResult<bool>> ApproveCompanyAsync(Guid companyId);
    Task<ServiceObjectResult<bool>> RejectCompanyAsync(Guid companyId);
    Task<ServiceObjectResult<bool>> SuspendCompanyAsync(Guid companyId);
    Task<ServiceObjectResult<bool>> BanCompanyAsync(Guid companyId);
}
