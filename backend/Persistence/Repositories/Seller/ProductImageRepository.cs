using Domain.Entities.Seller;
using NArchitecture.Core.Persistence.Repositories;
using Persistence.Contexts;
using Persistence.IRepositories.Seller;

namespace Persistence.Repositories.Seller;

public class ProductImageRepository : EfRepositoryBase<ProductImage, Guid, BaseDbContext>, IProductImageRepository
{
    public ProductImageRepository(BaseDbContext context) : base(context)
    {
    }
}