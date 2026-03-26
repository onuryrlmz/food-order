using Domain.Entities.Buyer;
using NArchitecture.Core.Persistence.Repositories;

namespace Persistence.IRepositories.Buyer;

public interface ISupportActionRepository : IAsyncRepository<SupportAction, Guid>, IRepository<SupportAction, Guid>
{
}
