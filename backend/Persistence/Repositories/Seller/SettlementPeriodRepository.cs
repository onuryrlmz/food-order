using Domain.Entities.Seller;
using NArchitecture.Core.Persistence.Repositories;
using Persistence.Contexts;
using Persistence.IRepositories.Seller;

namespace Persistence.Repositories.Seller;

public class SettlementPeriodRepository : EfRepositoryBase<SettlementPeriod, Guid, BaseDbContext>, ISettlementPeriodRepository
{
    public SettlementPeriodRepository(BaseDbContext context) : base(context)
    {
    }
}
