using Domain.Entities.Buyer;
using NArchitecture.Core.Persistence.Repositories;

namespace Persistence.IRepositories.Buyer;

public interface INotificationRepository : IAsyncRepository<Notification, Guid>, IRepository<Notification, Guid>
{
}
