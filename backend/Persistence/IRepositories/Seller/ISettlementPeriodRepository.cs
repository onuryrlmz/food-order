using Domain.Entities.Seller;
using NArchitecture.Core.Persistence.Repositories;

namespace Persistence.IRepositories.Seller;

public interface ISettlementPeriodRepository : IAsyncRepository<SettlementPeriod, Guid>, IRepository<SettlementPeriod, Guid>
{
}
