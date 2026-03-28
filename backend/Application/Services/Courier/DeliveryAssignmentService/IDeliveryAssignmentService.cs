using Domain.Dto.Courier;
using Domain.Dto.Seller.Courier;
using Domain.Service;

namespace Application.Services.Courier.DeliveryAssignmentService;

public interface IDeliveryAssignmentService
{
    // Courier actions
    Task<ServiceObjectResult<DeliveryAssignmentResponseDto>> GetActiveAssignment();
    Task<ServiceCollectionResult> GetAssignmentHistory(int page = 1, int pageSize = 20);
    Task<ServiceObjectResult<bool>> AcceptAssignment(Guid assignmentId);
    Task<ServiceObjectResult<bool>> RejectAssignment(Guid assignmentId, string? reason = null);
    Task<ServiceObjectResult<bool>> MarkPickedUp(Guid assignmentId);
    Task<ServiceObjectResult<bool>> MarkDelivered(Guid assignmentId);

    // Courier agreement actions
    Task<ServiceCollectionResult> GetMyAgreements();
    Task<ServiceObjectResult<bool>> AcceptAgreement(Guid agreementId);
    Task<ServiceObjectResult<bool>> RejectAgreementByC(Guid agreementId);
    Task<ServiceObjectResult<bool>> TerminateAgreementByC(Guid agreementId);

    // Restaurant/System actions
    Task<ServiceObjectResult<DeliveryAssignmentResponseDto>> CreateAssignment(Guid orderId);
    Task<ServiceObjectResult<bool>> AssignManually(Guid restaurantId, Guid orderId, Guid courierId);
    Task<ServiceObjectResult<bool>> CancelAssignment(Guid assignmentId, string reason);

    // Seller
    Task<ServiceCollectionResult> GetActiveDeliveries(Guid restaurantId);
    Task<ServiceCollectionResult> GetAgreements(Guid restaurantId);
    Task<ServiceObjectResult<AgreementResponseDto>> CreateAgreement(Guid restaurantId, CreateAgreementRequestDto requestDto);
    Task<ServiceObjectResult<AgreementResponseDto>> CreateAgreementByEmail(Guid restaurantId, CreateCourierAgreementByEmailDto requestDto);
    Task<ServiceObjectResult<bool>> UpdateAgreement(Guid agreementId, CreateAgreementRequestDto requestDto);
    Task<ServiceObjectResult<bool>> TerminateAgreement(Guid agreementId);
    Task<ServiceObjectResult<bool>> UpdateDeliverySettings(Guid restaurantId, UpdateDeliverySettingsRequestDto requestDto);

    // Customer tracking
    Task<ServiceObjectResult<CourierTrackingResponseDto>> GetOrderTracking(Guid orderId);
}