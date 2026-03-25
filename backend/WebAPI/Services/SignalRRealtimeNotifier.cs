using Application.Services.Common.NotificationService;
using Microsoft.AspNetCore.SignalR;
using WebAPI.Hubs;

namespace WebAPI.Services;

public class SignalRRealtimeNotifier : IRealtimeNotifier
{
    private readonly IHubContext<OrderHub> _orderHubContext;
    private readonly IHubContext<RestaurantHub> _restaurantHubContext;
    private readonly IHubContext<CourierHub> _courierHubContext;

    public SignalRRealtimeNotifier(
        IHubContext<OrderHub> orderHubContext,
        IHubContext<RestaurantHub> restaurantHubContext,
        IHubContext<CourierHub> courierHubContext)
    {
        _orderHubContext = orderHubContext;
        _restaurantHubContext = restaurantHubContext;
        _courierHubContext = courierHubContext;
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

    public async Task NotifyCourierNewAssignment(Guid courierId, Guid assignmentId, Guid orderId)
    {
        await _courierHubContext.Clients.Group($"courier-{courierId}")
            .SendAsync("NewAssignment", new { assignmentId, orderId, timestamp = DateTime.UtcNow });
    }

    public async Task BroadcastAssignmentToPool(Guid restaurantId, Guid assignmentId, Guid orderId)
    {
        await _courierHubContext.Clients.Group($"courier-pool-{restaurantId}")
            .SendAsync("NewAssignment", new { assignmentId, orderId, timestamp = DateTime.UtcNow });
    }

    public async Task NotifyCourierLocationUpdate(Guid orderId, decimal latitude, decimal longitude)
    {
        await _orderHubContext.Clients.Group($"order-{orderId}")
            .SendAsync("CourierLocationUpdate", new { latitude, longitude, timestamp = DateTime.UtcNow });
    }

}
