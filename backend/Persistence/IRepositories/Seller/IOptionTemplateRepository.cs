using Domain.Entities.Seller;
using NArchitecture.Core.Persistence.Repositories;

namespace Persistence.IRepositories.Seller;

public interface IOptionTemplateRepository : IAsyncRepository<OptionTemplate, Guid>, IRepository<OptionTemplate, Guid>
{
}
