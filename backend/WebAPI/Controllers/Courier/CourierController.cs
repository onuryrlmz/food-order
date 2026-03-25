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
    [AuthorizeAPIRequest(true, false, AuthorizationServiceEnums.UserRoleEnums.User)]
    public async Task<ServiceObjectResult<CourierProfileResponseDto>> Register([FromBody] RegisterCourierRequestDto requestDto)
        => await _courierService.Register(requestDto);

    [HttpGet("profile")]
    [AuthorizeAPIRequest(true, false, AuthorizationServiceEnums.UserRoleEnums.Courier)]
    public async Task<ServiceObjectResult<CourierProfileResponseDto>> GetProfile()
        => await _courierService.GetProfile();

    [HttpPut("profile")]
    [AuthorizeAPIRequest(true, false, AuthorizationServiceEnums.UserRoleEnums.Courier)]
    public async Task<ServiceObjectResult<bool>> UpdateProfile([FromBody] UpdateCourierProfileRequestDto requestDto)
        => await _courierService.UpdateProfile(requestDto);

    [HttpPost("go-online")]
    [AuthorizeAPIRequest(true, false, AuthorizationServiceEnums.UserRoleEnums.Courier)]
    public async Task<ServiceObjectResult<bool>> GoOnline()
        => await _courierService.GoOnline();

    [HttpPost("go-offline")]
    [AuthorizeAPIRequest(true, false, AuthorizationServiceEnums.UserRoleEnums.Courier)]
    public async Task<ServiceObjectResult<bool>> GoOffline()
        => await _courierService.GoOffline();

    [HttpPut("location")]
    [AuthorizeAPIRequest(true, false, AuthorizationServiceEnums.UserRoleEnums.Courier)]
    public async Task<ServiceObjectResult<bool>> UpdateLocation([FromBody] UpdateLocationRequestDto requestDto)
        => await _courierService.UpdateLocation(requestDto);

    [HttpGet("assignment/active")]
    [AuthorizeAPIRequest(true, false, AuthorizationServiceEnums.UserRoleEnums.Courier)]
    public async Task<ServiceObjectResult<DeliveryAssignmentResponseDto>> GetActiveAssignment()
        => await _deliveryAssignmentService.GetActiveAssignment();

    [HttpGet("assignment/history")]
    [AuthorizeAPIRequest(true, false, AuthorizationServiceEnums.UserRoleEnums.Courier)]
    public async Task<ServiceCollectionResult> GetAssignmentHistory([FromQuery] int page = 1, [FromQuery] int pageSize = 20)
        => await _deliveryAssignmentService.GetAssignmentHistory(page, pageSize);

    [HttpPost("assignment/{assignmentId}/accept")]
    [AuthorizeAPIRequest(true, false, AuthorizationServiceEnums.UserRoleEnums.Courier)]
    public async Task<ServiceObjectResult<bool>> AcceptAssignment(Guid assignmentId)
        => await _deliveryAssignmentService.AcceptAssignment(assignmentId);

    [HttpPost("assignment/{assignmentId}/reject")]
    [AuthorizeAPIRequest(true, false, AuthorizationServiceEnums.UserRoleEnums.Courier)]
    public async Task<ServiceObjectResult<bool>> RejectAssignment(Guid assignmentId, [FromQuery] string? reason = null)
        => await _deliveryAssignmentService.RejectAssignment(assignmentId, reason);

    [HttpPost("assignment/{assignmentId}/picked-up")]
    [AuthorizeAPIRequest(true, false, AuthorizationServiceEnums.UserRoleEnums.Courier)]
    public async Task<ServiceObjectResult<bool>> MarkPickedUp(Guid assignmentId)
        => await _deliveryAssignmentService.MarkPickedUp(assignmentId);

    [HttpPost("assignment/{assignmentId}/delivered")]
    [AuthorizeAPIRequest(true, false, AuthorizationServiceEnums.UserRoleEnums.Courier)]
    public async Task<ServiceObjectResult<bool>> MarkDelivered(Guid assignmentId)
        => await _deliveryAssignmentService.MarkDelivered(assignmentId);

    [HttpGet("earnings")]
    [AuthorizeAPIRequest(true, false, AuthorizationServiceEnums.UserRoleEnums.Courier)]
    public async Task<ServiceCollectionResult> GetEarnings([FromQuery] DateTime? from = null, [FromQuery] DateTime? to = null, [FromQuery] int page = 1, [FromQuery] int pageSize = 20)
        => await _courierEarningService.GetMyEarnings(from, to, page, pageSize);

    [HttpGet("earnings/summary")]
    [AuthorizeAPIRequest(true, false, AuthorizationServiceEnums.UserRoleEnums.Courier)]
    public async Task<ServiceObjectResult<EarningSummaryResponseDto>> GetEarningSummary()
        => await _courierEarningService.GetEarningSummary();
}
