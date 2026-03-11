using Domain.Entities.Seller;
using NArchitecture.Core.Persistence.Repositories;
using Persistence.Contexts;
using Persistence.IRepositories.Seller;

namespace Persistence.Repositories.Seller;

public class CategoryRepository : EfRepositoryBase<Category, Guid, BaseDbContext>, ICategoryRepository
{
    public CategoryRepository(BaseDbContext context) : base(context)
    {
    }
}