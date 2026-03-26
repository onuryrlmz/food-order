namespace Domain.Dto.Buyer.Notification;

public class UpdateNotificationPreferenceRequestDto
{
    public short NotificationTypeId { get; set; }
    public bool IsEnabled { get; set; }
}
