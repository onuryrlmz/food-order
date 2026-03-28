using Domain.Entities.Buyer;
using NArchitecture.Core.Persistence.Repositories;
using Persistence.Contexts;
using Persistence.IRepositories.Buyer;

namespace Persistence.Repositories.Buyer;

public class PaymentLogRepository : EfRepositoryBase<PaymentLog, Guid, BaseDbContext>, IPaymentLogRepository
{
    public PaymentLogRepository(BaseDbContext context) : base(context)
    {
    }
}
