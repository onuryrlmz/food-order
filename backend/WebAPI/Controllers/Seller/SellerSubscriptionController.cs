using Application.Services.Seller.SubscriptionService;
using Base.Enums;
using Domain.Dto.Seller.Subscription;
using Domain.Service;
using Microsoft.AspNetCore.Mvc;
using WebAPI.Helpers;

namespace WebAPI.Controllers.Seller;

[Route("v1/seller/subscription")]
[ApiController]
public class SellerSubscriptionController : BaseController
{
    private readonly ISubscriptionService _subscriptionService;

    public SellerSubscriptionController(ISubscriptionService subscriptionService) => _subscriptionService = subscriptionService;

    [HttpGet("plans")]
    public async Task<ServiceCollectionResult<GetSubscriptionPlanResponseDto>> GetPlans()
        => await _subscriptionService.GetPlans();

    [HttpGet("my")]
    [AuthorizeAPIRequest(true, false,
        AuthorizationServiceEnums.UserRoleEnums.SellerAdmin,
        AuthorizationServiceEnums.UserRoleEnums.SellerUser)]
    public async Task<ServiceCollectionResult<GetSubscriptionResponseDto>> GetMySubscriptions()
        => await _subscriptionService.GetSellerSubscriptions();

    [HttpGet("{restaurantId}/active")]
    [AuthorizeAPIRequest(true, false,
        AuthorizationServiceEnums.UserRoleEnums.SellerAdmin,
        AuthorizationServiceEnums.UserRoleEnums.SellerUser)]
    public async Task<ServiceObjectResult<GetSubscriptionResponseDto>> GetActive(Guid restaurantId)
        => await _subscriptionService.GetActiveSubscription(restaurantId);

    [HttpPost("subscribe")]
    [AuthorizeAPIRequest(true, false, AuthorizationServiceEnums.UserRoleEnums.SellerAdmin)]
    public async Task<ServiceObjectResult<Guid>> Subscribe([FromBody] SubscribeRequestDto requestDto)
        => await _subscriptionService.Subscribe(requestDto);

    [HttpPost("{id}/cancel")]
    [AuthorizeAPIRequest(true, false, AuthorizationServiceEnums.UserRoleEnums.SellerAdmin)]
    public async Task<ServiceObjectResult<bool>> Cancel(Guid id)
        => await _subscriptionService.CancelSubscription(id);

    [HttpGet("usage")]
    [AuthorizeAPIRequest(true, false,
        AuthorizationServiceEnums.UserRoleEnums.SellerAdmin,
        AuthorizationServiceEnums.UserRoleEnums.SellerUser)]
    public async Task<ServiceObjectResult<SubscriptionUsageDto>> GetUsage(
        [FromQuery] Guid restaurantId)
        => await _subscriptionService.GetUsageAsync(Client!._tokenDto!.SellerId ?? Guid.Empty, restaurantId);

    [HttpGet("upgrade/preview")]
    [AuthorizeAPIRequest(true, false,
        AuthorizationServiceEnums.UserRoleEnums.SellerAdmin)]
    public async Task<ServiceObjectResult<UpgradePreviewDto>> GetUpgradePreview(
        [FromQuery] Guid restaurantId, [FromQuery] Guid planId)
        => await _subscriptionService.GetUpgradePreviewAsync(Client!._tokenDto!.SellerId ?? Guid.Empty, restaurantId, planId);

    [HttpPost("upgrade")]
    [AuthorizeAPIRequest(true, false,
        AuthorizationServiceEnums.UserRoleEnums.SellerAdmin)]
    public async Task<ServiceObjectResult<bool>> Upgrade(
        [FromBody] UpgradeRequestDto request)
        => await _subscriptionService.UpgradePlanAsync(Client!._tokenDto!.SellerId ?? Guid.Empty, request);
}
