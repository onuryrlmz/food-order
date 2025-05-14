using Domain.Entities.Seller;
using NArchitecture.Core.Persistence.Repositories;
using Persistence.Contexts;
using Persistence.IRepositories.Seller;

namespace Persistence.Repositories.Seller;

public class MenuOptionValueRepository : EfRepositoryBase<MenuOptionValue, Guid, BaseDbContext>, IMenuOptionValueRepository
{
    public MenuOptionValueRepository(BaseDbContext context) : base(context)
    {
    }
}