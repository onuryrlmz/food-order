using Domain.Dto.Seller.Subscription;
using Domain.Service;

namespace Application.Services.Seller.SubscriptionService;

public interface ISubscriptionService
{
    Task<ServiceCollectionResult<GetSubscriptionPlanResponseDto>> GetPlans();
    Task<ServiceObjectResult<Guid>> CreatePlan(CreateSubscriptionPlanDto requestDto);
    Task<ServiceObjectResult<bool>> UpdatePlan(Guid planId, CreateSubscriptionPlanDto requestDto);
    Task<ServiceObjectResult<Guid>> Subscribe(SubscribeRequestDto requestDto);
    Task<ServiceObjectResult<bool>> CancelSubscription(Guid subscriptionId);
    Task<ServiceObjectResult<GetSubscriptionResponseDto>> GetActiveSubscription(Guid restaurantId);
    Task<ServiceCollectionResult<GetSubscriptionResponseDto>> GetSellerSubscriptions();
    Task<ServiceCollectionResult<GetSubscriptionResponseDto>> GetAllSubscriptions(int page = 1, int pageSize = 20);
    Task<ServiceObjectResult<bool>> CheckAndExpireSubscriptions();

    // Subscription Usage
    Task<ServiceObjectResult<SubscriptionUsageDto>> GetUsageAsync(Guid sellerId, Guid restaurantId);
    Task<ServiceObjectResult<UpgradePreviewDto>> GetUpgradePreviewAsync(Guid sellerId, Guid restaurantId, Guid targetPlanId);
    Task<ServiceObjectResult<bool>> UpgradePlanAsync(Guid sellerId, UpgradeRequestDto request);
    Task<ServiceObjectResult<bool>> IncrementOrderCountAsync(Guid restaurantId);
}
