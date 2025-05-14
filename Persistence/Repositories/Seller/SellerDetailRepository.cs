using Domain.Entities.Seller;
using NArchitecture.Core.Persistence.Repositories;
using Persistence.Contexts;
using Persistence.IRepositories.Seller;

namespace Persistence.Repositories.Seller;

public class SellerDetailRepository : EfRepositoryBase<SellerDetail, Guid, BaseDbContext>, ISellerDetailRepository
{
    public SellerDetailRepository(BaseDbContext context) : base(context)
    {
    }
}