using Domain.Entities.Seller;
using NArchitecture.Core.Persistence.Repositories;

namespace Persistence.IRepositories.Seller;

public interface IProductRepository : IAsyncRepository<Product, Guid>, IRepository<Product, Guid>
{
}