using NArchitecture.Core.Persistence.Repositories;

namespace Persistence.IRepositories.Courier;

public interface ICourierLocationHistoryRepository : IAsyncRepository<Domain.Entities.Courier.CourierLocationHistory, Guid>, IRepository<Domain.Entities.Courier.CourierLocationHistory, Guid>
{
}
