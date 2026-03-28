using Domain.Entities.Buyer;
using NArchitecture.Core.Persistence.Repositories;

namespace Persistence.IRepositories.Buyer;

public interface IPaymentLogRepository : IAsyncRepository<PaymentLog, Guid>, IRepository<PaymentLog, Guid>
{
}
