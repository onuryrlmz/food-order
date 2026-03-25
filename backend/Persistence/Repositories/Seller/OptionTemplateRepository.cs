using Domain.Entities.Seller;
using NArchitecture.Core.Persistence.Repositories;
using Persistence.Contexts;
using Persistence.IRepositories.Seller;

namespace Persistence.Repositories.Seller;

public class OptionTemplateRepository : EfRepositoryBase<OptionTemplate, Guid, BaseDbContext>, IOptionTemplateRepository
{
    public OptionTemplateRepository(BaseDbContext context) : base(context)
    {
    }
}