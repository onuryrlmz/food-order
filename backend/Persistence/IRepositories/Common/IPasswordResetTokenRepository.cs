using Domain.Entities.Common;
using NArchitecture.Core.Persistence.Repositories;

namespace Persistence.IRepositories.Common;

public interface IPasswordResetTokenRepository : IAsyncRepository<PasswordResetToken, Guid>, IRepository<PasswordResetToken, Guid>
{
}
