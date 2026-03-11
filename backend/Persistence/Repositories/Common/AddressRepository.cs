using Domain.Entities.Common;
using NArchitecture.Core.Persistence.Repositories;
using Persistence.Contexts;
using Persistence.IRepositories.Common;

namespace Persistence.Repositories.Common;

public class AddressRepository : EfRepositoryBase<Address, Guid, BaseDbContext>, IAddressRepository
{
    public AddressRepository(BaseDbContext context) : base(context)
    {
    }
}