using Domain.Dto.Buyer.Support;
using Domain.Service;

namespace Application.Services.Buyer.AiSupportService;

public interface IAiSupportService
{
    Task<ServiceObjectResult<CreateTicketResponseDto>> CreateTicket(CreateTicketRequestDto requestDto);
    Task<ServiceObjectResult<SupportMessageDto>> SendMessage(Guid ticketId, SendMessageRequestDto requestDto);
    Task<ServiceObjectResult<SupportTicketDetailDto>> GetTicketDetail(Guid ticketId);
    Task<ServiceCollectionResult<SupportTicketDto>> GetMyTickets(short? statusId = null, int page = 1, int pageSize = 20);
    Task<ServiceObjectResult<bool>> CloseTicket(Guid ticketId);
    Task<ServiceObjectResult<bool>> RateTicket(Guid ticketId, RateTicketRequestDto requestDto);
    Task<ServiceCollectionResult<SupportTicketDto>> GetEscalatedTickets(int page = 1, int pageSize = 20);
    Task<ServiceObjectResult<bool>> ApproveAction(Guid actionId);

    // Admin
    Task<ServiceCollectionResult<SupportTicketDto>> GetAllTickets(short? statusId = null, short? topicId = null, int page = 1, int pageSize = 20);
    Task<ServiceObjectResult<SupportTicketDetailDto>> GetTicketDetailAdmin(Guid ticketId);
    Task<ServiceObjectResult<SupportStatsDto>> GetSupportStats();
    Task<ServiceObjectResult<bool>> RejectAction(Guid actionId);
}
