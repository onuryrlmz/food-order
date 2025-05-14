using Domain.Entities.Seller;
using NArchitecture.Core.Persistence.Repositories;

namespace Persistence.IRepositories.Seller;

public interface ICategoryDetailRepository : IAsyncRepository<CategoryDetail, Guid>, IRepository<CategoryDetail, Guid>
{
}