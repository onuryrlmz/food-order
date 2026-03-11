using Domain.Entities.Seller;
using NArchitecture.Core.Persistence.Repositories;
using Persistence.Contexts;
using Persistence.IRepositories.Seller;

namespace Persistence.Repositories.Seller;

public class CategoryDetailRepository : EfRepositoryBase<CategoryDetail, Guid, BaseDbContext>, ICategoryDetailRepository
{
    public CategoryDetailRepository(BaseDbContext context) : base(context)
    {
    }
}