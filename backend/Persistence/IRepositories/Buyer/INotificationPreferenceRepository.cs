using Domain.Entities.Buyer;
using NArchitecture.Core.Persistence.Repositories;

namespace Persistence.IRepositories.Buyer;

public interface INotificationPreferenceRepository : IAsyncRepository<NotificationPreference, Guid>, IRepository<NotificationPreference, Guid>
{
}
