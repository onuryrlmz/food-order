using Application.Services.Buyer.AiSupportService;
using Base.Enums;
using Domain.Dto.Buyer.Support;
using Domain.Service;
using Microsoft.AspNetCore.Mvc;
using WebAPI.Helpers;

namespace WebAPI.Controllers.Customer;

[Route("v1/customer/support")]
[ApiController]
public class CustomerSupportController : BaseController
{
    private readonly IAiSupportService _supportService;

    public CustomerSupportController(IAiSupportService supportService)
    {
        _supportService = supportService;
    }

    [HttpPost("ticket")]
    [AuthorizeAPIRequest(true, false, UserRoleEnums.User)]
    public async Task<ServiceObjectResult<CreateTicketResponseDto>> CreateTicket([FromBody] CreateTicketRequestDto requestDto)
    {
        return await _supportService.CreateTicket(requestDto);
    }

    [HttpPost("ticket/{ticketId}/message")]
    [AuthorizeAPIRequest(true, false, UserRoleEnums.User)]
    public async Task<ServiceObjectResult<SupportMessageDto>> SendMessage(Guid ticketId, [FromBody] SendMessageRequestDto requestDto)
    {
        return await _supportService.SendMessage(ticketId, requestDto);
    }

    [HttpGet("ticket/{ticketId}")]
    [AuthorizeAPIRequest(true, false, UserRoleEnums.User)]
    public async Task<ServiceObjectResult<SupportTicketDetailDto>> GetTicketDetail(Guid ticketId)
    {
        return await _supportService.GetTicketDetail(ticketId);
    }

    [HttpGet("tickets")]
    [AuthorizeAPIRequest(true, false, UserRoleEnums.User)]
    public async Task<ServiceCollectionResult<SupportTicketDto>> GetMyTickets(
        [FromQuery] short? statusId,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20)
    {
        return await _supportService.GetMyTickets(statusId, page, pageSize);
    }

    [HttpPost("ticket/{ticketId}/close")]
    [AuthorizeAPIRequest(true, false, UserRoleEnums.User)]
    public async Task<ServiceObjectResult<bool>> CloseTicket(Guid ticketId)
    {
        return await _supportService.CloseTicket(ticketId);
    }

    [HttpPost("ticket/{ticketId}/rate")]
    [AuthorizeAPIRequest(true, false, UserRoleEnums.User)]
    public async Task<ServiceObjectResult<bool>> RateTicket(Guid ticketId, [FromBody] RateTicketRequestDto requestDto)
    {
        return await _supportService.RateTicket(ticketId, requestDto);
    }
}
