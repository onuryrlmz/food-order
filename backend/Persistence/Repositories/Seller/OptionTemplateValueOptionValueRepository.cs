using Domain.Entities.Seller;
using NArchitecture.Core.Persistence.Repositories;
using Persistence.Contexts;
using Persistence.IRepositories.Seller;

namespace Persistence.Repositories.Seller;

public class OptionTemplateValueOptionValueRepository : EfRepositoryBase<OptionTemplateValueOptionValue, Guid, BaseDbContext>, IOptionTemplateValueOptionValueRepository
{
    public OptionTemplateValueOptionValueRepository(BaseDbContext context) : base(context)
    {
    }
}