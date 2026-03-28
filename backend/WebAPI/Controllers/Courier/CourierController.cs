using Application.Services.Courier.CourierService;
using Application.Services.Courier.DeliveryAssignmentService;
using Application.Services.Courier.CourierEarningService;
using Base.Enums;
using Domain.Dto.Courier;
using Domain.Service;
using Microsoft.AspNetCore.Mvc;
using WebAPI.Helpers;

namespace WebAPI.Controllers.Courier;

[Route("v1/courier")]
[ApiController]
public class CourierController : BaseController
{
    private readonly ICourierService _courierService;
    private readonly IDeliveryAssignmentService _deliveryAssignmentService;
    private readonly ICourierEarningService _courierEarningService;

    public CourierController(
        ICourierService courierService,
        IDeliveryAssignmentService deliveryAssignmentService,
        ICourierEarningService courierEarningService)
    {
        _courierService = courierService;
        _deliveryAssignmentService = deliveryAssignmentService;
        _courierEarningService = courierEarningService;
    }

    [HttpPost("register")]
    [AuthorizeAPIRequest(true, false, UserRoleEnums.User)]
    public async Task<ServiceObjectResult<CourierProfileResponseDto>> Register([FromBody] RegisterCourierRequestDto requestDto)
    {
        return await _courierService.Register(requestDto);
    }

    [HttpGet("profile")]
    [AuthorizeAPIRequest(true, false, UserRoleEnums.Courier)]
    public async Task<ServiceObjectResult<CourierProfileResponseDto>> GetProfile()
    {
        return await _courierService.GetProfile();
    }

    [HttpPut("profile")]
    [AuthorizeAPIRequest(true, false, UserRoleEnums.Courier)]
    public async Task<ServiceObjectResult<bool>> UpdateProfile([FromBody] UpdateCourierProfileRequestDto requestDto)
    {
        return await _courierService.UpdateProfile(requestDto);
    }

    [HttpPost("go-online")]
    [AuthorizeAPIRequest(true, false, UserRoleEnums.Courier)]
    public async Task<ServiceObjectResult<bool>> GoOnline()
    {
        return await _courierService.GoOnline();
    }

    [HttpPost("go-offline")]
    [AuthorizeAPIRequest(true, false, UserRoleEnums.Courier)]
    public async Task<ServiceObjectResult<bool>> GoOffline()
    {
        return await _courierService.GoOffline();
    }

    [HttpPut("location")]
    [AuthorizeAPIRequest(true, false, UserRoleEnums.Courier)]
    public async Task<ServiceObjectResult<bool>> UpdateLocation([FromBody] UpdateLocationRequestDto requestDto)
    {
        return await _courierService.UpdateLocation(requestDto);
    }

    [HttpGet("assignment/active")]
    [AuthorizeAPIRequest(true, false, UserRoleEnums.Courier)]
    public async Task<ServiceObjectResult<DeliveryAssignmentResponseDto>> GetActiveAssignment()
    {
        return await _deliveryAssignmentService.GetActiveAssignment();
    }

    [HttpGet("assignment/history")]
    [AuthorizeAPIRequest(true, false, UserRoleEnums.Courier)]
    public async Task<ServiceCollectionResult> GetAssignmentHistory([FromQuery] int page = 1, [FromQuery] int pageSize = 20)
    {
        return await _deliveryAssignmentService.GetAssignmentHistory(page, pageSize);
    }

    [HttpPost("assignment/{assignmentId}/accept")]
    [AuthorizeAPIRequest(true, false, UserRoleEnums.Courier)]
    public async Task<ServiceObjectResult<bool>> AcceptAssignment(Guid assignmentId)
    {
        return await _deliveryAssignmentService.AcceptAssignment(assignmentId);
    }

    [HttpPost("assignment/{assignmentId}/reject")]
    [AuthorizeAPIRequest(true, false, UserRoleEnums.Courier)]
    public async Task<ServiceObjectResult<bool>> RejectAssignment(Guid assignmentId, [FromQuery] string? reason = null)
    {
        return await _deliveryAssignmentService.RejectAssignment(assignmentId, reason);
    }

    [HttpPost("assignment/{assignmentId}/picked-up")]
    [AuthorizeAPIRequest(true, false, UserRoleEnums.Courier)]
    public async Task<ServiceObjectResult<bool>> MarkPickedUp(Guid assignmentId)
    {
        return await _deliveryAssignmentService.MarkPickedUp(assignmentId);
    }

    [HttpPost("assignment/{assignmentId}/delivered")]
    [AuthorizeAPIRequest(true, false, UserRoleEnums.Courier)]
    public async Task<ServiceObjectResult<bool>> MarkDelivered(Guid assignmentId)
    {
        return await _deliveryAssignmentService.MarkDelivered(assignmentId);
    }

    [HttpGet("agreements")]
    [AuthorizeAPIRequest(true, false, UserRoleEnums.Courier)]
    public async Task<ServiceCollectionResult> GetMyAgreements()
    {
        return await _deliveryAssignmentService.GetMyAgreements();
    }

    [HttpPost("agreements/{agreementId}/accept")]
    [AuthorizeAPIRequest(true, false, UserRoleEnums.Courier)]
    public async Task<ServiceObjectResult<bool>> AcceptAgreement(Guid agreementId)
    {
        return await _deliveryAssignmentService.AcceptAgreement(agreementId);
    }

    [HttpPost("agreements/{agreementId}/reject")]
    [AuthorizeAPIRequest(true, false, UserRoleEnums.Courier)]
    public async Task<ServiceObjectResult<bool>> RejectAgreement(Guid agreementId)
    {
        return await _deliveryAssignmentService.RejectAgreementByC(agreementId);
    }

    [HttpPost("agreements/{agreementId}/terminate")]
    [AuthorizeAPIRequest(true, false, UserRoleEnums.Courier)]
    public async Task<ServiceObjectResult<bool>> TerminateAgreementByCourier(Guid agreementId)
    {
        return await _deliveryAssignmentService.TerminateAgreementByC(agreementId);
    }

    [HttpGet("earnings")]
    [AuthorizeAPIRequest(true, false, UserRoleEnums.Courier)]
    public async Task<ServiceCollectionResult> GetEarnings([FromQuery] DateTime? from = null, [FromQuery] DateTime? to = null, [FromQuery] int page = 1, [FromQuery] int pageSize = 20)
    {
        return await _courierEarningService.GetMyEarnings(from, to, page, pageSize);
    }

    [HttpGet("earnings/summary")]
    [AuthorizeAPIRequest(true, false, UserRoleEnums.Courier)]
    public async Task<ServiceObjectResult<EarningSummaryResponseDto>> GetEarningSummary()
    {
        return await _courierEarningService.GetEarningSummary();
    }
}