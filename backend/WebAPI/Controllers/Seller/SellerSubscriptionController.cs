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

    public SellerSubscriptionController(ISubscriptionService subscriptionService)
    {
        _subscriptionService = subscriptionService;
    }

    [HttpGet("plans")]
    public async Task<ServiceCollectionResult<GetSubscriptionPlanResponseDto>> GetPlans()
    {
        return await _subscriptionService.GetPlans();
    }

    [HttpGet("my")]
    [AuthorizeAPIRequest(true, false,
        UserRoleEnums.SellerAdmin,
        UserRoleEnums.SellerUser)]
    public async Task<ServiceCollectionResult<GetSubscriptionResponseDto>> GetMySubscriptions()
    {
        return await _subscriptionService.GetSellerSubscriptions();
    }

    [HttpGet("{restaurantId}/active")]
    [AuthorizeAPIRequest(true, false,
        UserRoleEnums.SellerAdmin,
        UserRoleEnums.SellerUser)]
    public async Task<ServiceObjectResult<GetSubscriptionResponseDto>> GetActive(Guid restaurantId)
    {
        return await _subscriptionService.GetActiveSubscription(restaurantId);
    }

    [HttpPost("subscribe")]
    [AuthorizeAPIRequest(true, false, UserRoleEnums.SellerAdmin)]
    public async Task<ServiceObjectResult<Guid>> Subscribe([FromBody] SubscribeRequestDto requestDto)
    {
        return await _subscriptionService.Subscribe(requestDto);
    }

    [HttpPost("{id}/cancel")]
    [AuthorizeAPIRequest(true, false, UserRoleEnums.SellerAdmin)]
    public async Task<ServiceObjectResult<bool>> Cancel(Guid id)
    {
        return await _subscriptionService.CancelSubscription(id);
    }

    [HttpGet("usage")]
    [AuthorizeAPIRequest(true, false,
        UserRoleEnums.SellerAdmin,
        UserRoleEnums.SellerUser)]
    public async Task<ServiceObjectResult<SubscriptionUsageDto>> GetUsage(
        [FromQuery] Guid restaurantId)
    {
        return await _subscriptionService.GetUsageAsync(Client!._tokenDto!.SellerId ?? Guid.Empty, restaurantId);
    }

    [HttpGet("upgrade/preview")]
    [AuthorizeAPIRequest(true, false,
        UserRoleEnums.SellerAdmin)]
    public async Task<ServiceObjectResult<UpgradePreviewDto>> GetUpgradePreview(
        [FromQuery] Guid restaurantId, [FromQuery] Guid planId)
    {
        return await _subscriptionService.GetUpgradePreviewAsync(Client!._tokenDto!.SellerId ?? Guid.Empty, restaurantId, planId);
    }

    [HttpPost("upgrade")]
    [AuthorizeAPIRequest(true, false,
        UserRoleEnums.SellerAdmin)]
    public async Task<ServiceObjectResult<bool>> Upgrade(
        [FromBody] UpgradeRequestDto request)
    {
        return await _subscriptionService.UpgradePlanAsync(Client!._tokenDto!.SellerId ?? Guid.Empty, request);
    }

    [HttpPut("auto-renew")]
    [AuthorizeAPIRequest(true, false, UserRoleEnums.SellerAdmin)]
    public async Task<ServiceObjectResult<bool>> ToggleAutoRenew([FromQuery] Guid restaurantId, [FromQuery] bool enabled)
    {
        return await _subscriptionService.ToggleAutoRenewAsync(restaurantId, enabled);
    }
}