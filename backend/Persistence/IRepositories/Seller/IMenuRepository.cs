using Domain.Entities.Seller;
using NArchitecture.Core.Persistence.Repositories;

namespace Persistence.IRepositories.Seller;

public interface IMenuRepository : IAsyncRepository<Menu, Guid>, IRepository<Menu, Guid>
{
}