using Domain.Entities.Seller;
using NArchitecture.Core.Persistence.Repositories;
using Persistence.Contexts;
using Persistence.IRepositories.Seller;

namespace Persistence.Repositories.Seller;

public class ProductAttributeRepository : EfRepositoryBase<ProductAttribute, Guid, BaseDbContext>, IProductAttributeRepository
{
    public ProductAttributeRepository(BaseDbContext context) : base(context)
    {
    }
}