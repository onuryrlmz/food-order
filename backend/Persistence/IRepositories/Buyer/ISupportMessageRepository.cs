using Domain.Entities.Buyer;
using NArchitecture.Core.Persistence.Repositories;

namespace Persistence.IRepositories.Buyer;

public interface ISupportMessageRepository : IAsyncRepository<SupportMessage, Guid>, IRepository<SupportMessage, Guid>
{
}
