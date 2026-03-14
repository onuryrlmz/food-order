using Domain.Entities.Buyer;
using NArchitecture.Core.Persistence.Repositories;

namespace Persistence.IRepositories.Buyer;

public interface IPaymentRepository : IAsyncRepository<Payment, Guid>, IRepository<Payment, Guid>
{
}
