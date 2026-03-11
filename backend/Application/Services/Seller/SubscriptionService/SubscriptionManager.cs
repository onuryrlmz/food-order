using Application.Services.Common.TokenService;
using Base.Enums;
using Domain.Dto.Seller.Subscription;
using Domain.Entities.Seller;
using Domain.Service;
using Persistence.IRepositories;
using Persistence.IRepositories.Seller;

namespace Application.Services.Seller.SubscriptionService;

public class SubscriptionManager : ISubscriptionService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ITokenAccessor _tokenAccessor;
    private readonly IRestaurantRepository _restaurantRepository;

    public SubscriptionManager(IUnitOfWork unitOfWork, ITokenAccessor tokenAccessor, IRestaurantRepository restaurantRepository)
    {
        _unitOfWork = unitOfWork;
        _tokenAccessor = tokenAccessor;
        _restaurantRepository = restaurantRepository;
    }

    public async Task<ServiceCollectionResult<GetSubscriptionPlanResponseDto>> GetPlans()
    {
        var result = new ServiceCollectionResult<GetSubscriptionPlanResponseDto>();
        try
        {
            var plans = await _unitOfWork.SubscriptionPlanRepository.GetListAsync(x => x.IsActive, size: 100);
            var dtos = plans.Items.Select(MapPlanToDto).ToList();
            result.SetData(dtos);
        }
        catch (Exception e)
        {
            result.Fail(e);
        }

        return result;
    }

    public async Task<ServiceObjectResult<Guid>> CreatePlan(CreateSubscriptionPlanDto requestDto)
    {
        var result = new ServiceObjectResult<Guid>();
        try
        {
            var plan = new SubscriptionPlan
            {
                Id = Guid.NewGuid(),
                Name = requestDto.Name,
                Description = requestDto.Description,
                PlanType = requestDto.PlanType,
                MonthlyPrice = requestDto.MonthlyPrice,
                MaxRestaurants = requestDto.MaxRestaurants,
                IsActive = true
            };

            await _unitOfWork.SubscriptionPlanRepository.AddAsync(plan);
            await _unitOfWork.CompleteAsync();

            result.SetData(plan.Id);
        }
        catch (Exception e)
        {
            result.Fail(e);
        }

        return result;
    }

    public async Task<ServiceObjectResult<bool>> UpdatePlan(Guid planId, CreateSubscriptionPlanDto requestDto)
    {
        var result = new ServiceObjectResult<bool>();
        try
        {
            var plan = await _unitOfWork.SubscriptionPlanRepository.GetAsync(x => x.Id == planId, enableTracking: true);
            if (plan == null)
            {
                result.Fail("Plan bulunamadı.");
                return result;
            }

            plan.Name = requestDto.Name;
            plan.Description = requestDto.Description;
            plan.PlanType = requestDto.PlanType;
            plan.MonthlyPrice = requestDto.MonthlyPrice;
            plan.MaxRestaurants = requestDto.MaxRestaurants;

            _unitOfWork.SubscriptionPlanRepository.Update(plan);
            await _unitOfWork.CompleteAsync();

            result.SetData(true);
        }
        catch (Exception e)
        {
            result.Fail(e);
        }

        return result;
    }

    public async Task<ServiceObjectResult<Guid>> Subscribe(SubscribeRequestDto requestDto)
    {
        var result = new ServiceObjectResult<Guid>();
        try
        {
            var token = _tokenAccessor.GetToken();
            if (token?.SellerId == null)
            {
                result.Fail("Kimlik doğrulama hatası.");
                return result;
            }

            var restaurant = await _restaurantRepository.GetAsync(x => x.Id == requestDto.RestaurantId && x.SellerId == token.SellerId);
            if (restaurant == null)
            {
                result.Fail("Restoran bulunamadı veya bu restorana erişim yetkiniz yok.");
                return result;
            }

            var plan = await _unitOfWork.SubscriptionPlanRepository.GetAsync(x => x.Id == requestDto.SubscriptionPlanId && x.IsActive);
            if (plan == null)
            {
                result.Fail("Abonelik planı bulunamadı.");
                return result;
            }

            // Mevcut aktif aboneliği iptal et
            var activeSubscription = await _unitOfWork.SubscriptionRepository.GetAsync(x =>
                x.RestaurantId == requestDto.RestaurantId &&
                x.StatusId == (short)AuthorizationServiceEnums.SubscriptionStatusEnums.Active,
                enableTracking: true);

            if (activeSubscription != null)
            {
                activeSubscription.StatusId = (short)AuthorizationServiceEnums.SubscriptionStatusEnums.Cancelled;
                _unitOfWork.SubscriptionRepository.Update(activeSubscription);
            }

            var now = DateTime.UtcNow;
            var subscription = new Subscription
            {
                Id = Guid.NewGuid(),
                SellerId = token.SellerId.Value,
                RestaurantId = requestDto.RestaurantId,
                SubscriptionPlanId = requestDto.SubscriptionPlanId,
                StatusId = (short)AuthorizationServiceEnums.SubscriptionStatusEnums.Active,
                StartDate = now,
                EndDate = now.AddMonths(1),
                PaidAmount = plan.MonthlyPrice
            };

            await _unitOfWork.SubscriptionRepository.AddAsync(subscription);

            // Restoranı aktif et
            var trackedRestaurant = await _restaurantRepository.GetAsync(x => x.Id == requestDto.RestaurantId, enableTracking: true);
            if (trackedRestaurant != null)
            {
                trackedRestaurant.IsActive = true;
                _restaurantRepository.Update(trackedRestaurant);
            }

            await _unitOfWork.CompleteAsync();

            result.SetData(subscription.Id);
        }
        catch (Exception e)
        {
            result.Fail(e);
        }

        return result;
    }

    public async Task<ServiceObjectResult<bool>> CancelSubscription(Guid subscriptionId)
    {
        var result = new ServiceObjectResult<bool>();
        try
        {
            var token = _tokenAccessor.GetToken();
            if (token?.SellerId == null)
            {
                result.Fail("Kimlik doğrulama hatası.");
                return result;
            }

            var subscription = await _unitOfWork.SubscriptionRepository.GetAsync(
                x => x.Id == subscriptionId && x.SellerId == token.SellerId.Value,
                enableTracking: true);

            if (subscription == null)
            {
                result.Fail("Abonelik bulunamadı.");
                return result;
            }

            subscription.StatusId = (short)AuthorizationServiceEnums.SubscriptionStatusEnums.Cancelled;
            _unitOfWork.SubscriptionRepository.Update(subscription);

            // Aktif abonelik kalmadıysa restoranı deaktif et
            var hasOtherActive = await _unitOfWork.SubscriptionRepository.AnyAsync(x =>
                x.RestaurantId == subscription.RestaurantId &&
                x.Id != subscriptionId &&
                x.StatusId == (short)AuthorizationServiceEnums.SubscriptionStatusEnums.Active &&
                x.EndDate >= DateTime.UtcNow);

            if (!hasOtherActive)
            {
                var restaurant = await _restaurantRepository.GetAsync(x => x.Id == subscription.RestaurantId, enableTracking: true);
                if (restaurant != null)
                {
                    restaurant.IsActive = false;
                    _restaurantRepository.Update(restaurant);
                }
            }

            await _unitOfWork.CompleteAsync();
            result.SetData(true);
        }
        catch (Exception e)
        {
            result.Fail(e);
        }

        return result;
    }

    public async Task<ServiceObjectResult<GetSubscriptionResponseDto>> GetActiveSubscription(Guid restaurantId)
    {
        var result = new ServiceObjectResult<GetSubscriptionResponseDto>();
        try
        {
            var subscription = await _unitOfWork.SubscriptionRepository.GetAsync(x =>
                x.RestaurantId == restaurantId &&
                x.StatusId == (short)AuthorizationServiceEnums.SubscriptionStatusEnums.Active);

            if (subscription == null)
            {
                result.Fail("Aktif abonelik bulunamadı.");
                return result;
            }

            var plan = await _unitOfWork.SubscriptionPlanRepository.GetAsync(x => x.Id == subscription.SubscriptionPlanId);
            var restaurant = await _restaurantRepository.GetAsync(x => x.Id == restaurantId);

            result.SetData(MapSubscriptionToDto(subscription, plan?.Name ?? "", restaurant?.Name ?? "", plan?.MonthlyPrice ?? 0));
        }
        catch (Exception e)
        {
            result.Fail(e);
        }

        return result;
    }

    public async Task<ServiceCollectionResult<GetSubscriptionResponseDto>> GetSellerSubscriptions()
    {
        var result = new ServiceCollectionResult<GetSubscriptionResponseDto>();
        try
        {
            var token = _tokenAccessor.GetToken();
            if (token?.SellerId == null)
            {
                result.Fail("Kimlik doğrulama hatası.");
                return result;
            }

            var subscriptions = await _unitOfWork.SubscriptionRepository.GetListAsync(
                x => x.SellerId == token.SellerId.Value,
                orderBy: q => q.OrderByDescending(s => s.CreatedDate),
                size: 100);

            var planIds = subscriptions.Items.Select(s => s.SubscriptionPlanId).Distinct().ToList();
            var plans = await _unitOfWork.SubscriptionPlanRepository.GetListAsync(x => planIds.Contains(x.Id), size: planIds.Count);
            var planDict = plans.Items.ToDictionary(p => p.Id);

            var restaurantIds = subscriptions.Items.Select(s => s.RestaurantId).Distinct().ToList();
            var restaurants = await _restaurantRepository.GetListAsync(x => restaurantIds.Contains(x.Id), size: restaurantIds.Count);
            var restaurantDict = restaurants.Items.ToDictionary(r => r.Id, r => r.Name);

            var dtos = subscriptions.Items.Select(s =>
            {
                var plan = planDict.GetValueOrDefault(s.SubscriptionPlanId);
                return MapSubscriptionToDto(s, plan?.Name ?? "", restaurantDict.GetValueOrDefault(s.RestaurantId, ""), plan?.MonthlyPrice ?? 0);
            }).ToList();

            result.SetData(dtos);
        }
        catch (Exception e)
        {
            result.Fail(e);
        }

        return result;
    }

    public async Task<ServiceCollectionResult<GetSubscriptionResponseDto>> GetAllSubscriptions(int page = 1, int pageSize = 20)
    {
        var result = new ServiceCollectionResult<GetSubscriptionResponseDto>();
        try
        {
            pageSize = Math.Min(pageSize, 50);
            var subscriptions = await _unitOfWork.SubscriptionRepository.GetListAsync(
                orderBy: q => q.OrderByDescending(s => s.CreatedDate),
                index: page - 1,
                size: pageSize);

            var planIds = subscriptions.Items.Select(s => s.SubscriptionPlanId).Distinct().ToList();
            var plans = await _unitOfWork.SubscriptionPlanRepository.GetListAsync(x => planIds.Contains(x.Id), size: planIds.Count);
            var planDict = plans.Items.ToDictionary(p => p.Id);

            var restaurantIds = subscriptions.Items.Select(s => s.RestaurantId).Distinct().ToList();
            var restaurants = await _restaurantRepository.GetListAsync(x => restaurantIds.Contains(x.Id), size: restaurantIds.Count);
            var restaurantDict = restaurants.Items.ToDictionary(r => r.Id, r => r.Name);

            var dtos = subscriptions.Items.Select(s =>
            {
                var plan = planDict.GetValueOrDefault(s.SubscriptionPlanId);
                return MapSubscriptionToDto(s, plan?.Name ?? "", restaurantDict.GetValueOrDefault(s.RestaurantId, ""), plan?.MonthlyPrice ?? 0);
            }).ToList();

            result.SetData(dtos);
        }
        catch (Exception e)
        {
            result.Fail(e);
        }

        return result;
    }

    public async Task<ServiceObjectResult<bool>> CheckAndExpireSubscriptions()
    {
        var result = new ServiceObjectResult<bool>();
        try
        {
            var expiredSubscriptions = await _unitOfWork.SubscriptionRepository.GetListAsync(
                x => x.StatusId == (short)AuthorizationServiceEnums.SubscriptionStatusEnums.Active &&
                     x.EndDate < DateTime.UtcNow,
                size: 500,
                enableTracking: true);

            foreach (var subscription in expiredSubscriptions.Items)
            {
                subscription.StatusId = (short)AuthorizationServiceEnums.SubscriptionStatusEnums.Expired;
                _unitOfWork.SubscriptionRepository.Update(subscription);

                var hasOtherActive = await _unitOfWork.SubscriptionRepository.AnyAsync(x =>
                    x.RestaurantId == subscription.RestaurantId &&
                    x.Id != subscription.Id &&
                    x.StatusId == (short)AuthorizationServiceEnums.SubscriptionStatusEnums.Active &&
                    x.EndDate >= DateTime.UtcNow);

                if (!hasOtherActive)
                {
                    var restaurant = await _restaurantRepository.GetAsync(x => x.Id == subscription.RestaurantId, enableTracking: true);
                    if (restaurant != null)
                    {
                        restaurant.IsActive = false;
                        _restaurantRepository.Update(restaurant);
                    }
                }
            }

            await _unitOfWork.CompleteAsync();
            result.SetData(true);
        }
        catch (Exception e)
        {
            result.Fail(e);
        }

        return result;
    }

    private static GetSubscriptionPlanResponseDto MapPlanToDto(SubscriptionPlan plan) => new()
    {
        Id = plan.Id,
        Name = plan.Name,
        Description = plan.Description,
        PlanType = plan.PlanType,
        MonthlyPrice = plan.MonthlyPrice,
        MaxRestaurants = plan.MaxRestaurants
    };

    private static GetSubscriptionResponseDto MapSubscriptionToDto(Subscription s, string planName, string restaurantName, decimal monthlyPrice) => new()
    {
        Id = s.Id,
        RestaurantId = s.RestaurantId,
        RestaurantName = restaurantName,
        SubscriptionPlanId = s.SubscriptionPlanId,
        PlanName = planName,
        MonthlyPrice = monthlyPrice,
        StatusId = s.StatusId,
        StatusName = ((AuthorizationServiceEnums.SubscriptionStatusEnums)s.StatusId).ToString(),
        StartDate = s.StartDate,
        EndDate = s.EndDate
    };
}
