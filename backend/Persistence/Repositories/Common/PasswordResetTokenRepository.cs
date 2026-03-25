using Domain.Entities.Common;
using NArchitecture.Core.Persistence.Repositories;
using Persistence.Contexts;
using Persistence.IRepositories.Common;

namespace Persistence.Repositories.Common;

public class PasswordResetTokenRepository : EfRepositoryBase<PasswordResetToken, Guid, BaseDbContext>, IPasswordResetTokenRepository
{
    public PasswordResetTokenRepository(BaseDbContext context) : base(context)
    {
    }
}