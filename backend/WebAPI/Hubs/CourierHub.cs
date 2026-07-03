using Base.Enums;
using Microsoft.AspNetCore.SignalR;

namespace WebAPI.Hubs;

public class CourierHub : Hub
{
    public async Task JoinCourierGroup(string courierId)
    {
        var token = HubAuth.GetToken(Context);
        if (token == null || !Guid.TryParse(courierId, out var cid))
            return;

        // Kurye yalnızca kendi grubuna katılabilir (admin muaf).
        if (token.Role != UserRoleEnums.Admin && token.CourierId != cid)
            return;

        await Groups.AddToGroupAsync(Context.ConnectionId, $"courier-{courierId}");
    }

    public async Task LeaveCourierGroup(string courierId)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"courier-{courierId}");
    }

    public async Task JoinRestaurantCourierPool(string restaurantId)
    {
        var token = HubAuth.GetToken(Context);
        if (token == null || !Guid.TryParse(restaurantId, out _))
            return;

        // Kurye havuzu yalnızca kimliği doğrulanmış kuryelere (veya admin'e) açıktır.
        if (token.Role != UserRoleEnums.Admin && token.CourierId == null)
            return;

        await Groups.AddToGroupAsync(Context.ConnectionId, $"courier-pool-{restaurantId}");
    }

    public async Task LeaveRestaurantCourierPool(string restaurantId)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"courier-pool-{restaurantId}");
    }
}
