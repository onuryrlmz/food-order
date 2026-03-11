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
}
