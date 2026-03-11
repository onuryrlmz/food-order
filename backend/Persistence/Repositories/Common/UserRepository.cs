using Domain.Entities.Common;
using NArchitecture.Core.Persistence.Repositories;
using Persistence.Contexts;
using Persistence.IRepositories.Common;

namespace Persistence.Repositories.Common;

public class UserRepository : EfRepositoryBase<User, Guid, BaseDbContext>, IUserRepository
{
    public UserRepository(BaseDbContext context) : base(context)
    {
    }
}