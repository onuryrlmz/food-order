using Domain.Entities.Seller;
using NArchitecture.Core.Persistence.Repositories;
using Persistence.Contexts;
using Persistence.IRepositories.Seller;

namespace Persistence.Repositories.Seller;

public class SettlementItemRepository : EfRepositoryBase<SettlementItem, Guid, BaseDbContext>, ISettlementItemRepository
{
    public SettlementItemRepository(BaseDbContext context) : base(context)
    {
    }
}
