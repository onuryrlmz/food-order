using Application.Services.Buyer.AiSupportService;
using Base.Enums;
using Domain.Dto.Buyer.Support;
using Domain.Service;
using Microsoft.AspNetCore.Mvc;
using WebAPI.Helpers;

namespace WebAPI.Controllers.Admin;

[Route("v1/admin/support")]
[ApiController]
public class AdminSupportController : BaseController
{
    private readonly IAiSupportService _supportService;

    public AdminSupportController(IAiSupportService supportService)
    {
        _supportService = supportService;
    }

    [HttpGet("escalated")]
    [AuthorizeAPIRequest(true, false, UserRoleEnums.Admin)]
    public async Task<ServiceCollectionResult<SupportTicketDto>> GetEscalatedTickets(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20)
    {
        return await _supportService.GetEscalatedTickets(page, pageSize);
    }

    [HttpPost("action/{actionId}/approve")]
    [AuthorizeAPIRequest(true, false, UserRoleEnums.Admin)]
    public async Task<ServiceObjectResult<bool>> ApproveAction(Guid actionId)
    {
        return await _supportService.ApproveAction(actionId);
    }

    [HttpGet("tickets")]
    [AuthorizeAPIRequest(true, false, UserRoleEnums.Admin)]
    public async Task<ServiceCollectionResult<SupportTicketDto>> GetAllTickets(
        [FromQuery] short? statusId,
        [FromQuery] short? topicId,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20)
    {
        return await _supportService.GetAllTickets(statusId, topicId, page, pageSize);
    }

    [HttpGet("ticket/{ticketId}")]
    [AuthorizeAPIRequest(true, false, UserRoleEnums.Admin)]
    public async Task<ServiceObjectResult<SupportTicketDetailDto>> GetTicketDetail(Guid ticketId)
    {
        return await _supportService.GetTicketDetailAdmin(ticketId);
    }

    [HttpGet("stats")]
    [AuthorizeAPIRequest(true, false, UserRoleEnums.Admin)]
    public async Task<ServiceObjectResult<SupportStatsDto>> GetStats()
    {
        return await _supportService.GetSupportStats();
    }

    [HttpPost("action/{actionId}/reject")]
    [AuthorizeAPIRequest(true, false, UserRoleEnums.Admin)]
    public async Task<ServiceObjectResult<bool>> RejectAction(Guid actionId)
    {
        return await _supportService.RejectAction(actionId);
    }
}
