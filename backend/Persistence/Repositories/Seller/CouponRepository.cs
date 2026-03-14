using Domain.Entities.Seller;
using NArchitecture.Core.Persistence.Repositories;
using Persistence.Contexts;
using Persistence.IRepositories.Seller;

namespace Persistence.Repositories.Seller;

public class CouponRepository : EfRepositoryBase<Coupon, Guid, BaseDbContext>, ICouponRepository
{
    public CouponRepository(BaseDbContext context) : base(context)
    {
    }
}
