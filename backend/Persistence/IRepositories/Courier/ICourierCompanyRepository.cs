using Domain.Entities.Courier;
using NArchitecture.Core.Persistence.Repositories;

namespace Persistence.IRepositories.Courier;

public interface ICourierCompanyRepository : IAsyncRepository<CourierCompany, Guid>, IRepository<CourierCompany, Guid>
{
}
