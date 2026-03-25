using Microsoft.AspNetCore.SignalR;

namespace WebAPI.Hubs;

public class CourierHub : Hub
{
    public async Task JoinCourierGroup(string courierId)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, $"courier-{courierId}");
    }

    public async Task LeaveCourierGroup(string courierId)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"courier-{courierId}");
    }

    public async Task JoinRestaurantCourierPool(string restaurantId)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, $"courier-pool-{restaurantId}");
    }

    public async Task LeaveRestaurantCourierPool(string restaurantId)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"courier-pool-{restaurantId}");
    }
}