using Domain.Entities.Seller;
using NArchitecture.Core.Persistence.Repositories;

namespace Persistence.IRepositories.Seller;

public interface IProductAttributeRepository : IAsyncRepository<ProductAttribute, Guid>, IRepository<ProductAttribute, Guid>
{
}