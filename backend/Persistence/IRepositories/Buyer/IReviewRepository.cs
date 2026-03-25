using Domain.Entities.Buyer;
using NArchitecture.Core.Persistence.Repositories;

namespace Persistence.IRepositories.Buyer;

public interface IReviewRepository : IAsyncRepository<Review, Guid>, IRepository<Review, Guid>
{
}