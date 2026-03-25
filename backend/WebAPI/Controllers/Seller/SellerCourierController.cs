using Application.Services.Courier.CourierService;
using Application.Services.Courier.DeliveryAssignmentService;
using Base.Enums;
using Domain.Dto.Seller.Courier;
using Domain.Service;
using Microsoft.AspNetCore.Mvc;
using WebAPI.Helpers;

namespace WebAPI.Controllers.Seller;

[Route("v1/seller/courier")]
[ApiController]
public class SellerCourierController : BaseController
{
    private readonly IDeliveryAssignmentService _deliveryAssignmentService;
    private readonly ICourierService _courierService;

    public SellerCourierController(
        IDeliveryAssignmentService deliveryAssignmentService,
        ICourierService courierService)
    {
        _deliveryAssignmentService = deliveryAssignmentService;
        _courierService = courierService;
    }

    [HttpGet("restaurant/{restaurantId}/agreements")]
    [AuthorizeAPIRequest(true, false, AuthorizationServiceEnums.UserRoleEnums.SellerAdmin)]
    public async Task<ServiceCollectionResult> GetAgreements(Guid restaurantId)
    {
        return await _deliveryAssignmentService.GetAgreements(restaurantId);
    }

    [HttpPost("restaurant/{restaurantId}/agreements")]
    [AuthorizeAPIRequest(true, false, AuthorizationServiceEnums.UserRoleEnums.SellerAdmin)]
    public async Task<ServiceObjectResult<AgreementResponseDto>> CreateAgreement(Guid restaurantId, [FromBody] CreateAgreementRequestDto requestDto)
    {
        return await _deliveryAssignmentService.CreateAgreement(restaurantId, requestDto);
    }

    [HttpPut("agreements/{agreementId}")]
    [AuthorizeAPIRequest(true, false, AuthorizationServiceEnums.UserRoleEnums.SellerAdmin)]
    public async Task<ServiceObjectResult<bool>> UpdateAgreement(Guid agreementId, [FromBody] CreateAgreementRequestDto requestDto)
    {
        return await _deliveryAssignmentService.UpdateAgreement(agreementId, requestDto);
    }

    [HttpDelete("agreements/{agreementId}")]
    [AuthorizeAPIRequest(true, false, AuthorizationServiceEnums.UserRoleEnums.SellerAdmin)]
    public async Task<ServiceObjectResult<bool>> TerminateAgreement(Guid agreementId)
    {
        return await _deliveryAssignmentService.TerminateAgreement(agreementId);
    }

    [HttpGet("restaurant/{restaurantId}/couriers")]
    [AuthorizeAPIRequest(true, false, AuthorizationServiceEnums.UserRoleEnums.SellerAdmin)]
    public async Task<ServiceCollectionResult> GetAvailableCouriers(Guid restaurantId)
    {
        return await _courierService.GetAvailableCouriersForRestaurant(restaurantId);
    }

    [HttpPost("restaurant/{restaurantId}/assign/{orderId}")]
    [AuthorizeAPIRequest(true, false, AuthorizationServiceEnums.UserRoleEnums.SellerAdmin)]
    public async Task<ServiceObjectResult<bool>> AssignCourier(Guid restaurantId, Guid orderId, [FromQuery] Guid courierId)
    {
        return await _deliveryAssignmentService.AssignManually(restaurantId, orderId, courierId);
    }

    [HttpGet("restaurant/{restaurantId}/deliveries")]
    [AuthorizeAPIRequest(true, false, AuthorizationServiceEnums.UserRoleEnums.SellerAdmin, AuthorizationServiceEnums.UserRoleEnums.SellerUser)]
    public async Task<ServiceCollectionResult> GetActiveDeliveries(Guid restaurantId)
    {
        return await _deliveryAssignmentService.GetActiveDeliveries(restaurantId);
    }

    [HttpPut("restaurant/{restaurantId}/settings")]
    [AuthorizeAPIRequest(true, false, AuthorizationServiceEnums.UserRoleEnums.SellerAdmin)]
    public async Task<ServiceObjectResult<bool>> UpdateDeliverySettings(Guid restaurantId, [FromBody] UpdateDeliverySettingsRequestDto requestDto)
    {
        return await _deliveryAssignmentService.UpdateDeliverySettings(restaurantId, requestDto);
    }
}