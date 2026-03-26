using Domain.Entities.Buyer;
using NArchitecture.Core.Persistence.Repositories;
using Persistence.Contexts;
using Persistence.IRepositories.Buyer;

namespace Persistence.Repositories.Buyer;

public class TipRepository : EfRepositoryBase<Tip, Guid, BaseDbContext>, ITipRepository
{
    public TipRepository(BaseDbContext context) : base(context)
    {
    }
}
