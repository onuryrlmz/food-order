using Domain.Entities.Buyer;
using NArchitecture.Core.Persistence.Repositories;
using Persistence.Contexts;
using Persistence.IRepositories.Buyer;

namespace Persistence.Repositories.Buyer;

public class BasketItemValueRepository : EfRepositoryBase<BasketItemValue, Guid, BaseDbContext>, IBasketItemValueRepository
{
    public BasketItemValueRepository(BaseDbContext context) : base(context)
    {
    }
}