using Domain.Entities.Buyer;
using NArchitecture.Core.Persistence.Repositories;
using Persistence.Contexts;
using Persistence.IRepositories.Buyer;

namespace Persistence.Repositories.Buyer;

public class SupportActionRepository : EfRepositoryBase<SupportAction, Guid, BaseDbContext>, ISupportActionRepository
{
    public SupportActionRepository(BaseDbContext context) : base(context)
    {
    }
}
