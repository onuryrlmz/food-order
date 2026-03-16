using Application.Services.Common.NotificationService;
using Microsoft.AspNetCore.SignalR;
using WebAPI.Hubs;

namespace WebAPI.Services;

public class SignalRRealtimeNotifier : IRealtimeNotifier
{
    private readonly IHubContext<OrderHub> _orderHubContext;
    private readonly IHubContext<RestaurantHub> _restaurantHubContext;

    public SignalRRealtimeNotifier(
        IHubContext<OrderHub> orderHubContext,
        IHubContext<RestaurantHub> restaurantHubContext)
    {
        _orderHubContext = orderHubContext;
        _restaurantHubContext = restaurantHubContext;
    }

    public async Task NotifyOrderStatusChanged(Guid orderId, short statusId)
    {
        await _orderHubContext.Clients.Group($"order-{orderId}")
            .SendAsync("OrderStatusChanged", orderId, statusId, DateTime.UtcNow);
    }

    public async Task NotifyNewOrderToRestaurant(Guid restaurantId, Guid orderId, decimal totalPrice)
    {
        await _restaurantHubContext.Clients.Group($"restaurant-{restaurantId}")
            .SendAsync("NewOrder", new { orderId, amount = totalPrice, createdAt = DateTime.UtcNow });
    }

    public async Task NotifyCourierLocationUpdated(Guid orderId, decimal latitude, decimal longitude)
    {
        await _orderHubContext.Clients.Group($"order-{orderId}")
            .SendAsync("CourierLocationUpdated", orderId, latitude, longitude, DateTime.UtcNow);
    }
}
