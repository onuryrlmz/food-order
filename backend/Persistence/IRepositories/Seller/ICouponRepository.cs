using Domain.Entities.Seller;
using NArchitecture.Core.Persistence.Repositories;

namespace Persistence.IRepositories.Seller;

public interface ICouponRepository : IAsyncRepository<Coupon, Guid>, IRepository<Coupon, Guid>
{
}