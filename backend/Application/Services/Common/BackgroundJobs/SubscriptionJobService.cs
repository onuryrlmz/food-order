using Base.Enums;
using Domain.Entities.Seller;
using Infrastructure.Adapters.OneSignalAdapter;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Persistence.Contexts;
using Persistence.IRepositories;
using Persistence.IRepositories.Seller;

namespace Application.Services.Common.BackgroundJobs;

public class SubscriptionJobService : ISubscriptionJobService
{
    private readonly BaseDbContext _context;
    private readonly INotificationService _notificationService;
    private readonly ILogger<SubscriptionJobService> _logger;

    public SubscriptionJobService(
        BaseDbContext context,
        INotificationService notificationService,
        ILogger<SubscriptionJobService> logger)
    {
        _context = context;
        _notificationService = notificationService;
        _logger = logger;
    }

    public async Task CheckExpiredSubscriptions()
    {
        try
        {
            var expired = await _context.Set<Subscription>()
                .Where(s => s.StatusId == (short)AuthorizationServiceEnums.SubscriptionStatusEnums.Active
                            && s.EndDate < DateTime.UtcNow)
                .ToListAsync();

            foreach (var sub in expired)
            {
                sub.StatusId = (short)AuthorizationServiceEnums.SubscriptionStatusEnums.Expired;

                var hasOtherActive = await _context.Set<Subscription>()
                    .AnyAsync(s => s.RestaurantId == sub.RestaurantId
                                   && s.Id != sub.Id
                                   && s.StatusId == (short)AuthorizationServiceEnums.SubscriptionStatusEnums.Active
                                   && s.EndDate >= DateTime.UtcNow);

                if (!hasOtherActive)
                {
                    var restaurant = await _context.Set<Restaurant>()
                        .FirstOrDefaultAsync(r => r.Id == sub.RestaurantId);
                    if (restaurant != null)
                        restaurant.IsActive = false;
                }
            }

            await _context.SaveChangesAsync();
            _logger.LogInformation("Expired {Count} subscriptions", expired.Count);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking expired subscriptions");
        }
    }

    public async Task SendExpiryReminders()
    {
        try
        {
            var threeDaysFromNow = DateTime.UtcNow.AddDays(3);
            var expiringSoon = await _context.Set<Subscription>()
                .Where(s => s.StatusId == (short)AuthorizationServiceEnums.SubscriptionStatusEnums.Active
                            && s.EndDate <= threeDaysFromNow
                            && s.EndDate > DateTime.UtcNow)
                .ToListAsync();

            foreach (var sub in expiringSoon)
            {
                var sellerUser = await _context.Set<Domain.Entities.Common.User>()
                    .FirstOrDefaultAsync(u => u.SellerId == sub.SellerId);
                if (sellerUser != null)
                {
                    var daysLeft = (int)(sub.EndDate - DateTime.UtcNow).TotalDays;
                    await _notificationService.SendToUserAsync(sellerUser.Id,
                        "Abonelik Hatırlatma",
                        $"Aboneliğinizin süresi {daysLeft} gün içinde dolacak.");
                }
            }

            _logger.LogInformation("Sent expiry reminders for {Count} subscriptions", expiringSoon.Count);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sending expiry reminders");
        }
    }

    public async Task AutoRenewSubscriptions()
    {
        try
        {
            var tomorrow = DateTime.UtcNow.AddDays(1);
            var toRenew = await _context.Set<Subscription>()
                .Include(s => s.SubscriptionPlan)
                .Where(s => s.StatusId == (short)AuthorizationServiceEnums.SubscriptionStatusEnums.Active
                            && s.AutoRenew
                            && s.EndDate <= tomorrow
                            && s.EndDate > DateTime.UtcNow)
                .ToListAsync();

            foreach (var sub in toRenew)
                try
                {
                    // Check if seller has a stored card
                    var cardUserKey = await _context.UserExternalInfos
                        .Where(u => u.Provider == "iyzico" && u.Key == "CardUserKey")
                        .Join(_context.Set<Domain.Entities.Common.User>().Where(u => u.SellerId == sub.SellerId),
                            ext => ext.UserId, user => user.Id, (ext, user) => ext.Value)
                        .FirstOrDefaultAsync();

                    if (string.IsNullOrEmpty(cardUserKey))
                    {
                        // No stored card, notify seller
                        var sellerUser = await _context.Set<Domain.Entities.Common.User>().FirstOrDefaultAsync(u => u.SellerId == sub.SellerId);
                        if (sellerUser != null)
                            await _notificationService.SendToUserAsync(sellerUser.Id,
                                "Otomatik Yenileme Başarısız",
                                "Kayıtlı kart bulunamadığı için aboneliğiniz yenilenemedi.");
                        continue;
                    }

                    // Create new subscription period
                    var newSub = new Subscription
                    {
                        Id = Guid.NewGuid(),
                        SellerId = sub.SellerId,
                        RestaurantId = sub.RestaurantId,
                        SubscriptionPlanId = sub.SubscriptionPlanId,
                        StatusId = (short)AuthorizationServiceEnums.SubscriptionStatusEnums.Active,
                        StartDate = sub.EndDate,
                        EndDate = sub.EndDate.AddMonths(1),
                        PaidAmount = sub.SubscriptionPlan?.MonthlyPrice ?? sub.PaidAmount,
                        AutoRenew = true,
                        RenewalAttempts = 0
                    };

                    sub.StatusId = (short)AuthorizationServiceEnums.SubscriptionStatusEnums.Expired;
                    _context.Set<Subscription>().Add(newSub);

                    var sellerNotify = await _context.Set<Domain.Entities.Common.User>().FirstOrDefaultAsync(u => u.SellerId == sub.SellerId);
                    if (sellerNotify != null)
                        await _notificationService.SendToUserAsync(sellerNotify.Id,
                            "Abonelik Yenilendi",
                            $"Aboneliğiniz otomatik olarak yenilendi. Yeni bitiş tarihi: {newSub.EndDate:dd.MM.yyyy}");

                    _logger.LogInformation("Auto-renewed subscription {SubId} for restaurant {RestId}", sub.Id, sub.RestaurantId);
                }
                catch (Exception ex)
                {
                    sub.RenewalAttempts++;
                    sub.LastRenewalAttemptAt = DateTime.UtcNow;

                    if (sub.RenewalAttempts >= 3)
                    {
                        sub.StatusId = (short)AuthorizationServiceEnums.SubscriptionStatusEnums.Expired;
                        var restaurant = await _context.Set<Restaurant>().FirstOrDefaultAsync(r => r.Id == sub.RestaurantId);
                        if (restaurant != null)
                            restaurant.IsActive = false;
                    }

                    _logger.LogError(ex, "Failed to auto-renew subscription {SubId}", sub.Id);
                }

            await _context.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in auto-renewal job");
        }
    }

    public async Task CheckUsageWarnings()
    {
        try
        {
            var now = DateTime.UtcNow;
            var activeSubscriptions = await _context.Set<Subscription>()
                .Include(s => s.SubscriptionPlan)
                .Where(s => s.StatusId == (short)AuthorizationServiceEnums.SubscriptionStatusEnums.Active)
                .ToListAsync();

            foreach (var sub in activeSubscriptions)
            {
                if (sub.SubscriptionPlan == null || sub.SubscriptionPlan.MaxOrdersPerMonth == int.MaxValue)
                    continue;

                var usage = await _context.Set<SubscriptionUsage>()
                    .FirstOrDefaultAsync(u => u.SubscriptionId == sub.Id && u.Year == now.Year && u.Month == now.Month);

                if (usage == null) continue;

                var percent = (double)usage.OrderCount / sub.SubscriptionPlan.MaxOrdersPerMonth * 100;
                if (percent >= 80)
                {
                    var sellerUser = await _context.Set<Domain.Entities.Common.User>().FirstOrDefaultAsync(u => u.SellerId == sub.SellerId);
                    if (sellerUser != null)
                        await _notificationService.SendToUserAsync(sellerUser.Id,
                            "Kullanım Uyarısı",
                            $"Aylık sipariş limitinizin %{percent:F0}'ine ulaştınız. ({usage.OrderCount}/{sub.SubscriptionPlan.MaxOrdersPerMonth})");
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking usage warnings");
        }
    }
}