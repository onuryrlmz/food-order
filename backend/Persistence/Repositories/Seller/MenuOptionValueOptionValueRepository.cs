using Domain.Entities.Seller;
using NArchitecture.Core.Persistence.Repositories;
using Persistence.Contexts;
using Persistence.IRepositories.Seller;

namespace Persistence.Repositories.Seller;

public class MenuOptionValueOptionValueRepository : EfRepositoryBase<MenuOptionValueOptionValue, Guid, BaseDbContext>, IMenuOptionValueOptionValueRepository
{
    public MenuOptionValueOptionValueRepository(BaseDbContext context) : base(context)
    {
    }
}