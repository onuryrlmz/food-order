using Domain.Entities.Buyer;
using NArchitecture.Core.Persistence.Repositories;
using Persistence.Contexts;
using Persistence.IRepositories.Buyer;

namespace Persistence.Repositories.Buyer;

public class BasketItemValueItemValueRepository : EfRepositoryBase<BasketItemValueItemValue, Guid, BaseDbContext>, IBasketItemValueItemValueRepository
{
    public BasketItemValueItemValueRepository(BaseDbContext context) : base(context)
    {
    }
}