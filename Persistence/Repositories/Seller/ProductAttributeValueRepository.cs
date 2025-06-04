using Domain.Entities.Seller;
using NArchitecture.Core.Persistence.Repositories;
using Persistence.Contexts;
using Persistence.IRepositories.Seller;

namespace Persistence.Repositories.Seller;

public class ProductAttributeValueRepository : EfRepositoryBase<ProductAttributeValue, Guid, BaseDbContext>, IProductAttributeValueRepository
{
    public ProductAttributeValueRepository(BaseDbContext context) : base(context)
    {
    }
}