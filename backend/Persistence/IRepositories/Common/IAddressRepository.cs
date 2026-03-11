using Domain.Entities.Common;
using NArchitecture.Core.Persistence.Repositories;

namespace Persistence.IRepositories.Common;

public interface IAddressRepository : IAsyncRepository<Address, Guid>, IRepository<Address, Guid>
{
}