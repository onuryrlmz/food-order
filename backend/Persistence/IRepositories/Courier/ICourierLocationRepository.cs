using Domain.Entities.Courier;
using NArchitecture.Core.Persistence.Repositories;

namespace Persistence.IRepositories.Courier;

public interface ICourierLocationRepository : IAsyncRepository<CourierLocation, Guid>, IRepository<CourierLocation, Guid>
{
}
