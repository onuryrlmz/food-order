using NArchitecture.Core.Persistence.Repositories;
using Persistence.Contexts;
using Persistence.IRepositories.Seller;

namespace Persistence.Repositories.Seller;

public class SellerRepository : EfRepositoryBase<Domain.Entities.Seller.Seller, Guid, BaseDbContext>, ISellerRepository
{
    public SellerRepository(BaseDbContext context) : base(context)
    {
    }
}