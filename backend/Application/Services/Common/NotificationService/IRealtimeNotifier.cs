namespace Application.Services.Common.NotificationService;

public interface IRealtimeNotifier
{
    Task NotifyOrderStatusChanged(Guid orderId, short statusId);
    Task NotifyNewOrderToRestaurant(Guid restaurantId, Guid orderId, decimal totalPrice);
    Task NotifyCourierLocationUpdated(Guid orderId, decimal latitude, decimal longitude);
}
