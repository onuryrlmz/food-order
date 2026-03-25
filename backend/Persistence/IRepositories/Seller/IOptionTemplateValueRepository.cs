using Domain.Entities.Seller;
using NArchitecture.Core.Persistence.Repositories;

namespace Persistence.IRepositories.Seller;

public interface IOptionTemplateValueRepository : IAsyncRepository<OptionTemplateValue, Guid>, IRepository<OptionTemplateValue, Guid>
{
}