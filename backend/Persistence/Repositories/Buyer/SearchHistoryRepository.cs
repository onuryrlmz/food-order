using Domain.Entities.Buyer;
using NArchitecture.Core.Persistence.Repositories;
using Persistence.Contexts;
using Persistence.IRepositories.Buyer;

namespace Persistence.Repositories.Buyer;

public class SearchHistoryRepository : EfRepositoryBase<SearchHistory, Guid, BaseDbContext>, ISearchHistoryRepository
{
    public SearchHistoryRepository(BaseDbContext context) : base(context)
    {
    }
}
