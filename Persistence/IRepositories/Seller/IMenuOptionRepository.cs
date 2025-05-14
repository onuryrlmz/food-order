using Domain.Entities.Seller;
using NArchitecture.Core.Persistence.Repositories;

namespace Persistence.IRepositories.Seller;

public interface IMenuOptionRepository : IAsyncRepository<MenuOption, Guid>, IRepository<MenuOption, Guid>
{
}