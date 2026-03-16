using Domain.Entities.Courier;
using NArchitecture.Core.Persistence.Repositories;

namespace Persistence.IRepositories.Courier;

public interface ICourierCompanyMemberRepository : IAsyncRepository<CourierCompanyMember, Guid>, IRepository<CourierCompanyMember, Guid>
{
}
