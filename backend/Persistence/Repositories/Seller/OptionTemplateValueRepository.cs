using Domain.Entities.Seller;
using NArchitecture.Core.Persistence.Repositories;
using Persistence.Contexts;
using Persistence.IRepositories.Seller;

namespace Persistence.Repositories.Seller;

public class OptionTemplateValueRepository : EfRepositoryBase<OptionTemplateValue, Guid, BaseDbContext>, IOptionTemplateValueRepository
{
    public OptionTemplateValueRepository(BaseDbContext context) : base(context)
    {
    }
}
