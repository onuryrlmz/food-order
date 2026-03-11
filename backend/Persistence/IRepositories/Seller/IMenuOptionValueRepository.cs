using Domain.Entities.Seller;
using NArchitecture.Core.Persistence.Repositories;

namespace Persistence.IRepositories.Seller;

public interface IMenuOptionValueRepository : IAsyncRepository<MenuOptionValue, Guid>, IRepository<MenuOptionValue, Guid>
{
}