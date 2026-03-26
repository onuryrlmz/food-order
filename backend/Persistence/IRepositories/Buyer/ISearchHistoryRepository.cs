using Domain.Entities.Buyer;
using NArchitecture.Core.Persistence.Repositories;

namespace Persistence.IRepositories.Buyer;

public interface ISearchHistoryRepository : IAsyncRepository<SearchHistory, Guid>, IRepository<SearchHistory, Guid>
{
}
