using Microsoft.AspNetCore.SignalR;

namespace WebAPI.Hubs;

public class RestaurantHub : Hub
{
    public async Task JoinRestaurantGroup(string restaurantId)
        => await Groups.AddToGroupAsync(Context.ConnectionId, $"restaurant-{restaurantId}");

    public async Task LeaveRestaurantGroup(string restaurantId)
        => await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"restaurant-{restaurantId}");
}
