using Domain.Entities.Common;
using NArchitecture.Core.Persistence.Repositories;

namespace Persistence.IRepositories.Common;

public interface IUserRepository : IAsyncRepository<User, Guid>, IRepository<User, Guid>
{
}