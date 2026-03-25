using NArchitecture.Core.Persistence.Repositories;

namespace Persistence.IRepositories.Courier;

public interface ICourierRepository : IAsyncRepository<Domain.Entities.Courier.Courier, Guid>, IRepository<Domain.Entities.Courier.Courier, Guid>
{
}