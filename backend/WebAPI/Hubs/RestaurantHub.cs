using Base.Enums;
using Microsoft.AspNetCore.SignalR;

namespace WebAPI.Hubs;

public class RestaurantHub : Hub
{
    public async Task JoinRestaurantGroup(string restaurantId)
    {
        var token = HubAuth.GetToken(Context);
        if (token == null || !Guid.TryParse(restaurantId, out var rid))
            return;

        // Yalnızca restoranın satıcısı veya admin katılabilir.
        var isAdmin = token.Role == UserRoleEnums.Admin;
        var owns = token.RestaurantIds != null && token.RestaurantIds.Contains(rid);
        if (!isAdmin && !owns)
            return;

        await Groups.AddToGroupAsync(Context.ConnectionId, $"restaurant-{restaurantId}");
    }

    public async Task LeaveRestaurantGroup(string restaurantId)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"restaurant-{restaurantId}");
    }
}
