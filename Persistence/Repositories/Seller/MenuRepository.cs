using Domain.Entities.Seller;
using NArchitecture.Core.Persistence.Repositories;
using Persistence.Contexts;
using Persistence.IRepositories.Seller;

namespace Persistence.Repositories.Seller;

public class MenuRepository : EfRepositoryBase<Menu, Guid, BaseDbContext>, IMenuRepository
{
    public MenuRepository(BaseDbContext context) : base(context)
    {
    }
}