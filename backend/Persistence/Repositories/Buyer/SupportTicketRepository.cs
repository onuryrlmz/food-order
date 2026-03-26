using Domain.Entities.Buyer;
using NArchitecture.Core.Persistence.Repositories;
using Persistence.Contexts;
using Persistence.IRepositories.Buyer;

namespace Persistence.Repositories.Buyer;

public class SupportTicketRepository : EfRepositoryBase<SupportTicket, Guid, BaseDbContext>, ISupportTicketRepository
{
    public SupportTicketRepository(BaseDbContext context) : base(context)
    {
    }
}
