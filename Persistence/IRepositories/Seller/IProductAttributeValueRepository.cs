using Domain.Entities.Seller;
using NArchitecture.Core.Persistence.Repositories;

namespace Persistence.IRepositories.Seller;

public interface IProductAttributeValueRepository : IAsyncRepository<ProductAttributeValue, Guid>, IRepository<ProductAttributeValue, Guid>
{
}