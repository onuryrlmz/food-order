using Domain.Entities.Buyer;
using NArchitecture.Core.Persistence.Repositories;
using Persistence.Contexts;
using Persistence.IRepositories.Buyer;

namespace Persistence.Repositories.Buyer;

public class ReviewRepository : EfRepositoryBase<Review, Guid, BaseDbContext>, IReviewRepository
{
    public ReviewRepository(BaseDbContext context) : base(context)
    {
    }
}
