using Domain.Entities.Seller;
using NArchitecture.Core.Persistence.Repositories;

namespace Persistence.IRepositories.Seller;

public interface IMenuOptionValueOptionValueRepository : IAsyncRepository<MenuOptionValueOptionValue, Guid>, IRepository<MenuOptionValueOptionValue, Guid>
{
}