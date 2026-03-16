namespace Infrastructure.Adapters.OneSignalAdapter;

public interface INotificationService
{
    Task SendToUserAsync(Guid userId, string title, string message, Dictionary<string, string>? data = null);
    Task SendToUsersAsync(List<Guid> userIds, string title, string message, Dictionary<string, string>? data = null);
}
