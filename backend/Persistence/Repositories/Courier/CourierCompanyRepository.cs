using Domain.Entities.Courier;
using NArchitecture.Core.Persistence.Repositories;
using Persistence.Contexts;
using Persistence.IRepositories.Courier;

namespace Persistence.Repositories.Courier;

public class CourierCompanyRepository : EfRepositoryBase<CourierCompany, Guid, BaseDbContext>, ICourierCompanyRepository
{
    public CourierCompanyRepository(BaseDbContext context) : base(context)
    {
    }
}
