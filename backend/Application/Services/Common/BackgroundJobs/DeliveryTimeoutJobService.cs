using Base.Enums;
using Domain.Entities.Buyer;
using Domain.Entities.Seller;
using Infrastructure.Adapters.OneSignalAdapter;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Persistence.Contexts;

namespace Application.Services.Common.BackgroundJobs;

public class DeliveryTimeoutJobService : IDeliveryTimeoutJobService
{
    private readonly BaseDbContext _context;
    private readonly INotificationService _notificationService;
    private readonly ILogger<DeliveryTimeoutJobService> _logger;

    public DeliveryTimeoutJobService(
        BaseDbContext context,
        INotificationService notificationService,
        ILogger<DeliveryTimeoutJobService> logger)
    {
        _context = context;
        _notificationService = notificationService;
        _logger = logger;
    }

    public async Task CheckDeliveryTimeouts()
    {
        try
        {
            var now = DateTime.UtcNow;

            // Find OnTheWay orders where CreatedDate + MaxDeliveryTime < now
            var overdueOrders = await _context.Set<Order>()
                .Where(o => o.StatusId == (short)AuthorizationServiceEnums.OrderStatusEnums.OnTheWay
                            && o.DeletedDate == null)
                .Join(
                    _context.Set<Restaurant>(),
                    o => o.RestaurantId,
                    r => r.Id,
                    (o, r) => new { Order = o, Restaurant = r })
                .Where(x => x.Order.CreatedDate.AddMinutes(x.Restaurant.MaxDeliveryTime) < now)
                .ToListAsync();

            foreach (var item in overdueOrders)
            {
                // Notify customer
                await _notificationService.SendToUserAsync(
                    item.Order.UserId,
                    "Teslimat Gecikmesi",
                    $"Siparişinizin tahmini teslimat süresi aşıldı. Restoran: {item.Restaurant.Name}",
                    new Dictionary<string, string> { { "orderId", item.Order.Id.ToString() } });

                // Notify restaurant (seller admin)
                var sellerUser = await _context.Set<Domain.Entities.Common.User>()
                    .FirstOrDefaultAsync(u => u.SellerId == item.Restaurant.SellerId
                                              && u.UserRoleId == (short)AuthorizationServiceEnums.UserRoleEnums.SellerAdmin);

                if (sellerUser != null)
                    await _notificationService.SendToUserAsync(
                        sellerUser.Id,
                        "Teslimat Gecikmesi",
                        $"Sipariş #{item.Order.Id.ToString()[..8]} tahmini teslimat süresini aştı.",
                        new Dictionary<string, string> { { "orderId", item.Order.Id.ToString() } });

                _logger.LogWarning("Overdue delivery: Order {OrderId}, Restaurant {RestaurantName}",
                    item.Order.Id, item.Restaurant.Name);
            }

            _logger.LogInformation("Checked delivery timeouts: {OverdueCount} overdue orders found", overdueOrders.Count);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking delivery timeouts");
        }
    }
}