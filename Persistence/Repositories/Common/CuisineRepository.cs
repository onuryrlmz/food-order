using Domain.Entities.Common;
using NArchitecture.Core.Persistence.Repositories;
using Persistence.Contexts;
using Persistence.IRepositories.Common;

namespace Persistence.Repositories.Common;

public class CuisineRepository : EfRepositoryBase<Cuisine, Guid, BaseDbContext>, ICuisineRepository
{
    public CuisineRepository(BaseDbContext context) : base(context)
    {
    }
}