using Domain.Entities.Seller;
using NArchitecture.Core.Persistence.Repositories;
using Persistence.Contexts;
using Persistence.IRepositories.Seller;

namespace Persistence.Repositories.Seller;

public class OptionTemplateValueOptionRepository : EfRepositoryBase<OptionTemplateValueOption, Guid, BaseDbContext>, IOptionTemplateValueOptionRepository
{
    public OptionTemplateValueOptionRepository(BaseDbContext context) : base(context)
    {
    }
}
