using Application.Services.Common.TokenService;
using Base.Enums;
using Dapper;
using Domain.Dto.Seller.Subscription;
using Domain.Entities.Seller;
using Domain.Service;
using Microsoft.EntityFrameworkCore;
using Persistence.Contexts;
using Persistence.IRepositories;
using Persistence.IRepositories.Seller;

namespace Application.Services.Seller.SubscriptionService;

public class SubscriptionManager : ISubscriptionService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ITokenAccessor _tokenAccessor;
    private readonly IRestaurantRepository _restaurantRepository;
    private readonly BaseDbContext _context;

    public SubscriptionManager(IUnitOfWork unitOfWork, ITokenAccessor tokenAccessor, IRestaurantRepository restaurantRepository, BaseDbContext context)
    {
        _unitOfWork = unitOfWork;
        _tokenAccessor = tokenAccessor;
        _restaurantRepository = restaurantRepository;
        _context = context;
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

    public async Task<ServiceObjectResult<SubscriptionUsageDto>> GetUsageAsync(Guid sellerId, Guid restaurantId)
    {
        var result = new ServiceObjectResult<SubscriptionUsageDto>();
        try
        {
            var subscription = await _unitOfWork.SubscriptionRepository.GetAsync(
                s => s.RestaurantId == restaurantId && s.SellerId == sellerId &&
                     s.StatusId == (short)AuthorizationServiceEnums.SubscriptionStatusEnums.Active);

            if (subscription == null)
            {
                result.Fail("Aktif abonelik bulunamadı.");
                return result;
            }

            var plan = await _unitOfWork.SubscriptionPlanRepository.GetAsync(p => p.Id == subscription.SubscriptionPlanId);
            var now = DateTime.UtcNow;

            var usage = await _unitOfWork.SubscriptionUsageRepository.GetAsync(
                u => u.SubscriptionId == subscription.Id && u.Year == now.Year && u.Month == now.Month);

            var orderCount = usage?.OrderCount ?? 0;
            var maxOrders = plan?.MaxOrdersPerMonth ?? int.MaxValue;
            var usagePercent = maxOrders == int.MaxValue ? 0 : (double)orderCount / maxOrders * 100;
            var remainingDays = Math.Max(0, (int)(subscription.EndDate - now).TotalDays);

            result.SetData(new SubscriptionUsageDto
            {
                OrderCount = orderCount,
                MaxOrdersPerMonth = maxOrders,
                UsagePercentage = Math.Round(usagePercent, 1),
                RemainingDays = remainingDays,
                PlanName = plan?.Name ?? ""
            });
        }
        catch (Exception e)
        {
            result.Fail(e);
        }
        return result;
    }

    public async Task<ServiceObjectResult<UpgradePreviewDto>> GetUpgradePreviewAsync(Guid sellerId, Guid restaurantId, Guid targetPlanId)
    {
        var result = new ServiceObjectResult<UpgradePreviewDto>();
        try
        {
            var subscription = await _unitOfWork.SubscriptionRepository.GetAsync(
                s => s.RestaurantId == restaurantId && s.SellerId == sellerId &&
                     s.StatusId == (short)AuthorizationServiceEnums.SubscriptionStatusEnums.Active);

            if (subscription == null)
            {
                result.Fail("Aktif abonelik bulunamadı.");
                return result;
            }

            var currentPlan = await _unitOfWork.SubscriptionPlanRepository.GetAsync(p => p.Id == subscription.SubscriptionPlanId);
            var targetPlan = await _unitOfWork.SubscriptionPlanRepository.GetAsync(p => p.Id == targetPlanId && p.IsActive);

            if (targetPlan == null)
            {
                result.Fail("Hedef plan bulunamadı.");
                return result;
            }

            if (targetPlan.MonthlyPrice <= (currentPlan?.MonthlyPrice ?? 0))
            {
                result.Fail("Sadece daha yüksek bir plana geçiş yapabilirsiniz.");
                return result;
            }

            var now = DateTime.UtcNow;
            var totalDays = (int)(subscription.EndDate - subscription.StartDate).TotalDays;
            var remainingDays = Math.Max(0, (int)(subscription.EndDate - now).TotalDays);
            var proratedAmount = totalDays > 0
                ? (targetPlan.MonthlyPrice - (currentPlan?.MonthlyPrice ?? 0)) * remainingDays / totalDays
                : 0;

            result.SetData(new UpgradePreviewDto
            {
                CurrentPlanName = currentPlan?.Name ?? "",
                TargetPlanName = targetPlan.Name,
                CurrentPrice = currentPlan?.MonthlyPrice ?? 0,
                TargetPrice = targetPlan.MonthlyPrice,
                ProratedAmount = Math.Round(proratedAmount, 2),
                RemainingDays = remainingDays,
                TotalDays = totalDays,
                TargetMaxOrders = targetPlan.MaxOrdersPerMonth
            });
        }
        catch (Exception e)
        {
            result.Fail(e);
        }
        return result;
    }

    public async Task<ServiceObjectResult<bool>> UpgradePlanAsync(Guid sellerId, UpgradeRequestDto request)
    {
        var result = new ServiceObjectResult<bool>();
        try
        {
            var subscription = await _unitOfWork.SubscriptionRepository.GetAsync(
                s => s.RestaurantId == request.RestaurantId && s.SellerId == sellerId &&
                     s.StatusId == (short)AuthorizationServiceEnums.SubscriptionStatusEnums.Active,
                enableTracking: true);

            if (subscription == null)
            {
                result.Fail("Aktif abonelik bulunamadı.");
                return result;
            }

            var targetPlan = await _unitOfWork.SubscriptionPlanRepository.GetAsync(p => p.Id == request.TargetPlanId && p.IsActive);
            if (targetPlan == null)
            {
                result.Fail("Hedef plan bulunamadı.");
                return result;
            }

            subscription.SubscriptionPlanId = targetPlan.Id;
            _unitOfWork.SubscriptionRepository.Update(subscription);
            await _unitOfWork.CompleteAsync();

            result.SetData(true);
            result.AddSuccessMessage($"Plan '{targetPlan.Name}' olarak yükseltildi.");
        }
        catch (Exception e)
        {
            result.Fail(e);
        }
        return result;
    }

    public async Task<ServiceObjectResult<bool>> IncrementOrderCountAsync(Guid restaurantId)
    {
        var result = new ServiceObjectResult<bool>();
        try
        {
            var subscription = await _unitOfWork.SubscriptionRepository.GetAsync(
                s => s.RestaurantId == restaurantId &&
                     s.StatusId == (short)AuthorizationServiceEnums.SubscriptionStatusEnums.Active);

            if (subscription == null)
            {
                result.Fail("Aktif abonelik bulunamadı.");
                return result;
            }

            var plan = await _unitOfWork.SubscriptionPlanRepository.GetAsync(p => p.Id == subscription.SubscriptionPlanId);
            var now = DateTime.UtcNow;

            var conn = _context.Database.GetDbConnection();
            if (conn.State != System.Data.ConnectionState.Open)
                await conn.OpenAsync();

            // Lazy create usage record with ON DUPLICATE KEY UPDATE
            var usageId = Guid.NewGuid();
            await conn.ExecuteAsync(@"
                INSERT INTO SubscriptionUsage (Id, SubscriptionId, Year, Month, OrderCount, CreatedDate, UpdatedDate)
                VALUES (@id, @subscriptionId, @year, @month, 0, @now, @now)
                ON DUPLICATE KEY UPDATE OrderCount = OrderCount",
                new { id = usageId, subscriptionId = subscription.Id, year = now.Year, month = now.Month, now });

            // Atomic increment with limit check
            var affected = await conn.ExecuteAsync(@"
                UPDATE SubscriptionUsage
                SET OrderCount = OrderCount + 1, UpdatedDate = @now
                WHERE SubscriptionId = @subscriptionId
                  AND Year = @year AND Month = @month
                  AND OrderCount < @maxOrders",
                new
                {
                    subscriptionId = subscription.Id,
                    year = now.Year,
                    month = now.Month,
                    maxOrders = plan?.MaxOrdersPerMonth ?? int.MaxValue,
                    now
                });

            if (affected == 0)
            {
                result.Fail("Aylık sipariş limitinize ulaştınız. Paketinizi yükseltin.");
                return result;
            }

            result.SetData(true);
        }
        catch (Exception ex)
        {
            result.Fail($"Hata: {ex.Message}");
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
