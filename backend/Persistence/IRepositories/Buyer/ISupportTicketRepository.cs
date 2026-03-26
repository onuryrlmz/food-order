using Domain.Entities.Buyer;
using NArchitecture.Core.Persistence.Repositories;

namespace Persistence.IRepositories.Buyer;

public interface ISupportTicketRepository : IAsyncRepository<SupportTicket, Guid>, IRepository<SupportTicket, Guid>
{
}
