using Domain.Entities.Courier;
using NArchitecture.Core.Persistence.Repositories;
using Persistence.Contexts;
using Persistence.IRepositories.Courier;

namespace Persistence.Repositories.Courier;

public class CourierCompanyMemberRepository : EfRepositoryBase<CourierCompanyMember, Guid, BaseDbContext>, ICourierCompanyMemberRepository
{
    public CourierCompanyMemberRepository(BaseDbContext context) : base(context)
    {
    }
}
