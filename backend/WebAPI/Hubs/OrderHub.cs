using Base.Enums;
using Microsoft.AspNetCore.SignalR;
using Persistence.IRepositories;

namespace WebAPI.Hubs;

public class OrderHub : Hub
{
    private readonly IUnitOfWork _unitOfWork;

    public OrderHub(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task JoinOrderGroup(string orderId)
    {
        var token = HubAuth.GetToken(Context);
        if (token == null || !Guid.TryParse(orderId, out var oid))
            return;

        var order = await _unitOfWork.OrderRepository.GetAsync(x => x.Id == oid);
        if (order == null)
            return;

        // Yalnızca siparişin sahibi müşteri, siparişin restoranının satıcısı veya admin katılabilir.
        var isOwner = order.UserId == token.UserId;
        var isAdmin = token.Role == UserRoleEnums.Admin;
        var ownsRestaurant = token.RestaurantIds != null && token.RestaurantIds.Contains(order.RestaurantId);
        if (!isOwner && !isAdmin && !ownsRestaurant)
            return;

        await Groups.AddToGroupAsync(Context.ConnectionId, $"order-{orderId}");
    }

    public async Task LeaveOrderGroup(string orderId)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"order-{orderId}");
    }
}
