using Domain.Entities.Buyer;
using NArchitecture.Core.Persistence.Repositories;
using Persistence.Contexts;
using Persistence.IRepositories.Buyer;

namespace Persistence.Repositories.Buyer;

public class NotificationPreferenceRepository : EfRepositoryBase<NotificationPreference, Guid, BaseDbContext>, INotificationPreferenceRepository
{
    public NotificationPreferenceRepository(BaseDbContext context) : base(context)
    {
    }
}
