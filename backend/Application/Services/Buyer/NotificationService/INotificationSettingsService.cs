using Domain.Dto.Buyer.Notification;
using Domain.Service;

namespace Application.Services.Buyer.NotificationService;

public interface INotificationSettingsService
{
    Task<ServiceCollectionResult<NotificationDto>> GetNotifications(int page = 1, int pageSize = 20);
    Task<ServiceObjectResult<UnreadCountDto>> GetUnreadCount();
    Task<ServiceObjectResult<bool>> MarkAsRead(Guid notificationId);
    Task<ServiceObjectResult<bool>> MarkAllAsRead();
    Task<ServiceCollectionResult<NotificationPreferenceDto>> GetPreferences();
    Task<ServiceObjectResult<bool>> UpdatePreference(UpdateNotificationPreferenceRequestDto requestDto);
    Task<ServiceObjectResult<bool>> CreateNotification(Guid userId, short typeId, string title, string message, string? data = null, Guid? relatedOrderId = null);
}
