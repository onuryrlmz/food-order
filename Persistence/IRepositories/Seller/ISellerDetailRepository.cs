using Domain.Entities.Seller;
using NArchitecture.Core.Persistence.Repositories;

namespace Persistence.IRepositories.Seller;

public interface ISellerDetailRepository : IAsyncRepository<SellerDetail, Guid>, IRepository<SellerDetail, Guid>
{
}