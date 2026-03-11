using Domain.Entities.Seller;
using NArchitecture.Core.Persistence.Repositories;
using Persistence.Contexts;
using Persistence.IRepositories.Seller;

namespace Persistence.Repositories.Seller;

public class MenuOptionRepository : EfRepositoryBase<MenuOption, Guid, BaseDbContext>, IMenuOptionRepository
{
    public MenuOptionRepository(BaseDbContext context) : base(context)
    {
    }
}