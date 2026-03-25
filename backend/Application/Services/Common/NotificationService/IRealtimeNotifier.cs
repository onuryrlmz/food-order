namespace Application.Services.Common.NotificationService;

public interface IRealtimeNotifier
{
    Task NotifyOrderStatusChanged(Guid orderId, short statusId);
    Task NotifyNewOrderToRestaurant(Guid restaurantId, Guid orderId, decimal totalPrice);
    Task NotifyCourierNewAssignment(Guid courierId, Guid assignmentId, Guid orderId);
    Task BroadcastAssignmentToPool(Guid restaurantId, Guid assignmentId, Guid orderId);
    Task NotifyCourierLocationUpdate(Guid orderId, decimal latitude, decimal longitude);
}