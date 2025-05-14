using NArchitecture.Core.Persistence.Repositories;

namespace Persistence.IRepositories.Seller;

public interface ISellerRepository : IAsyncRepository<Domain.Entities.Seller.Seller, Guid>, IRepository<Domain.Entities.Seller.Seller, Guid>
{
}