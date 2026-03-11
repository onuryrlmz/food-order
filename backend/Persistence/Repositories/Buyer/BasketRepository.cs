using Domain.Entities.Buyer;
using NArchitecture.Core.Persistence.Repositories;
using Persistence.Contexts;
using Persistence.IRepositories.Buyer;

namespace Persistence.Repositories.Buyer;

public class BasketRepository : EfRepositoryBase<Basket, Guid, BaseDbContext>, IBasketRepository
{
    public BasketRepository(BaseDbContext context) : base(context)
    {
    }
}