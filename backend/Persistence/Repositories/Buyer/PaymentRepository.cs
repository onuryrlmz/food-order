using Domain.Entities.Buyer;
using NArchitecture.Core.Persistence.Repositories;
using Persistence.Contexts;
using Persistence.IRepositories.Buyer;

namespace Persistence.Repositories.Buyer;

public class PaymentRepository : EfRepositoryBase<Payment, Guid, BaseDbContext>, IPaymentRepository
{
    public PaymentRepository(BaseDbContext context) : base(context)
    {
    }
}
