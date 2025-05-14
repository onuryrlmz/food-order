using Domain.Entities.Seller;
using NArchitecture.Core.Persistence.Repositories;
using Persistence.Contexts;
using Persistence.IRepositories.Seller;

namespace Persistence.Repositories.Seller;

public class MenuOptionValueOptionRepository : EfRepositoryBase<MenuOptionValueOption, Guid, BaseDbContext>, IMenuOptionValueOptionRepository
{
    public MenuOptionValueOptionRepository(BaseDbContext context) : base(context)
    {
    }
}