using Domain.Entities.Buyer;
using NArchitecture.Core.Persistence.Repositories;
using Persistence.Contexts;
using Persistence.IRepositories.Buyer;

namespace Persistence.Repositories.Buyer;

public class SupportMessageRepository : EfRepositoryBase<SupportMessage, Guid, BaseDbContext>, ISupportMessageRepository
{
    public SupportMessageRepository(BaseDbContext context) : base(context)
    {
    }
}
