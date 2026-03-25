using Domain.Entities.Courier;
using NArchitecture.Core.Persistence.Repositories;

namespace Persistence.IRepositories.Courier;

public interface ICourierEarningRepository : IAsyncRepository<CourierEarning, Guid>, IRepository<CourierEarning, Guid>
{
}
