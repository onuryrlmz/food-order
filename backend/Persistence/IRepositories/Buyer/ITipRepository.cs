using Domain.Entities.Buyer;
using NArchitecture.Core.Persistence.Repositories;

namespace Persistence.IRepositories.Buyer;

public interface ITipRepository : IAsyncRepository<Tip, Guid>, IRepository<Tip, Guid>
{
}
