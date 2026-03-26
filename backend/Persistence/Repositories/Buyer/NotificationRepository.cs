using Domain.Entities.Buyer;
using NArchitecture.Core.Persistence.Repositories;
using Persistence.Contexts;
using Persistence.IRepositories.Buyer;

namespace Persistence.Repositories.Buyer;

public class NotificationRepository : EfRepositoryBase<Notification, Guid, BaseDbContext>, INotificationRepository
{
    public NotificationRepository(BaseDbContext context) : base(context)
    {
    }
}
