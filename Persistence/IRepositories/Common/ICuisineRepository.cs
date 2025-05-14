using Domain.Entities.Common;
using NArchitecture.Core.Persistence.Repositories;

namespace Persistence.IRepositories.Common;

public interface ICuisineRepository : IAsyncRepository<Cuisine, Guid>, IRepository<Cuisine, Guid>
{
}