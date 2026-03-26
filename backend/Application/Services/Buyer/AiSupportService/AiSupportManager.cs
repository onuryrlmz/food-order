using System.Data;
using System.Text.Json;
using Application.Services.Common.TokenService;
using Dapper;
using Domain.Dto.Buyer.Support;
using Domain.Entities.Buyer;
using Domain.Service;
using Infrastructure.Adapters.AiSupportAdapter;
using Microsoft.EntityFrameworkCore;
using Persistence.Contexts;
using Persistence.IRepositories;

namespace Application.Services.Buyer.AiSupportService;

public class AiSupportManager : IAiSupportService
{
    private const short StatusOpen = 1;
    private const short StatusInProgress = 2;
    private const short StatusResolved = 3;
    private const short StatusClosed = 4;
    private const short StatusEscalated = 5;

    private const short SenderCustomer = 1;
    private const short SenderAI = 2;

    private readonly IUnitOfWork _unitOfWork;
    private readonly ITokenAccessor _tokenAccessor;
    private readonly BaseDbContext _context;
    private readonly IAiSupportAdapter _aiAdapter;

    public AiSupportManager(IUnitOfWork unitOfWork, ITokenAccessor tokenAccessor,
        BaseDbContext context, IAiSupportAdapter aiAdapter)
    {
        _unitOfWork = unitOfWork;
        _tokenAccessor = tokenAccessor;
        _context = context;
        _aiAdapter = aiAdapter;
    }

    public async Task<ServiceObjectResult<CreateTicketResponseDto>> CreateTicket(CreateTicketRequestDto requestDto)
    {
        var result = new ServiceObjectResult<CreateTicketResponseDto>();
        try
        {
            var token = _tokenAccessor.GetToken();
            if (token == null)
            {
                result.Fail("Unauthorized");
                return result;
            }

            // Create ticket
            var ticket = new SupportTicket
            {
                Id = Guid.NewGuid(),
                UserId = token.UserId,
                OrderId = requestDto.OrderId,
                RestaurantId = requestDto.RestaurantId,
                TopicId = requestDto.TopicId,
                StatusId = StatusOpen,
                Subject = requestDto.Subject,
                IsEscalated = false
            };

            await _unitOfWork.SupportTicketRepository.AddAsync(ticket);

            // Save customer message
            var customerMessage = new SupportMessage
            {
                Id = Guid.NewGuid(),
                TicketId = ticket.Id,
                SenderType = SenderCustomer,
                Content = requestDto.InitialMessage
            };

            await _unitOfWork.SupportMessageRepository.AddAsync(customerMessage);
            await _unitOfWork.CompleteAsync();

            // Build AI context
            var aiContext = await BuildAiContext(ticket, new List<SupportMessage> { customerMessage });

            // Get AI response
            var aiResponse = await _aiAdapter.GetResponseAsync(aiContext);

            // Save AI message
            var aiMessage = new SupportMessage
            {
                Id = Guid.NewGuid(),
                TicketId = ticket.Id,
                SenderType = SenderAI,
                Content = aiResponse.Content,
                AiModelUsed = aiResponse.ModelUsed,
                TokensUsed = aiResponse.TokensUsed
            };

            await _unitOfWork.SupportMessageRepository.AddAsync(aiMessage);

            // Handle suggested action
            if (aiResponse.SuggestedAction != null)
            {
                var action = new SupportAction
                {
                    Id = Guid.NewGuid(),
                    TicketId = ticket.Id,
                    ActionType = aiResponse.SuggestedAction.ActionType,
                    ActionData = aiResponse.SuggestedAction.ActionDataJson,
                    IsApproved = false,
                    IsExecuted = false
                };
                await _unitOfWork.SupportActionRepository.AddAsync(action);
            }

            // Handle escalation
            if (aiResponse.ShouldEscalate)
            {
                ticket = await _unitOfWork.SupportTicketRepository.GetAsync(
                    x => x.Id == ticket.Id, enableTracking: true);
                if (ticket != null)
                {
                    ticket.IsEscalated = true;
                    ticket.StatusId = StatusEscalated;
                    await _unitOfWork.SupportTicketRepository.UpdateAsync(ticket);
                }
            }
            else
            {
                ticket = await _unitOfWork.SupportTicketRepository.GetAsync(
                    x => x.Id == ticket.Id, enableTracking: true);
                if (ticket != null)
                {
                    ticket.StatusId = StatusInProgress;
                    await _unitOfWork.SupportTicketRepository.UpdateAsync(ticket);
                }
            }

            await _unitOfWork.CompleteAsync();

            result.SetData(new CreateTicketResponseDto
            {
                TicketId = ticket!.Id,
                StatusId = ticket.StatusId,
                CreatedDate = ticket.CreatedDate,
                AiResponse = new SupportMessageDto
                {
                    Id = aiMessage.Id,
                    SenderType = SenderAI,
                    Content = aiMessage.Content,
                    CreatedDate = aiMessage.CreatedDate
                }
            });
        }
        catch (Exception e)
        {
            result.Fail(e);
        }

        return result;
    }

    public async Task<ServiceObjectResult<SupportMessageDto>> SendMessage(Guid ticketId, SendMessageRequestDto requestDto)
    {
        var result = new ServiceObjectResult<SupportMessageDto>();
        try
        {
            var token = _tokenAccessor.GetToken();
            if (token == null)
            {
                result.Fail("Unauthorized");
                return result;
            }

            var ticket = await _unitOfWork.SupportTicketRepository.GetAsync(
                x => x.Id == ticketId && x.UserId == token.UserId);
            if (ticket == null)
            {
                result.Fail("Ticket not found");
                return result;
            }

            if (ticket.StatusId == StatusClosed)
            {
                result.Fail("This ticket is closed");
                return result;
            }

            // Save customer message
            var customerMessage = new SupportMessage
            {
                Id = Guid.NewGuid(),
                TicketId = ticketId,
                SenderType = SenderCustomer,
                Content = requestDto.Content
            };

            await _unitOfWork.SupportMessageRepository.AddAsync(customerMessage);
            await _unitOfWork.CompleteAsync();

            // Load conversation history
            const string historyQuery = @"
                SELECT `Id`, `SenderType`, `Content`, `CreatedDate`
                FROM `SupportMessage`
                WHERE `TicketId` = @ticketId AND `DeletedDate` IS NULL
                ORDER BY `CreatedDate` ASC";

            var conn = _context.Database.GetDbConnection();
            if (conn.State != ConnectionState.Open)
                await conn.OpenAsync();

            var messages = (await conn.QueryAsync<SupportMessage>(historyQuery, new { ticketId })).ToList();

            // Build AI context
            var aiContext = await BuildAiContext(ticket, messages);

            // Get AI response
            var aiResponse = await _aiAdapter.GetResponseAsync(aiContext);

            // Save AI message
            var aiMessage = new SupportMessage
            {
                Id = Guid.NewGuid(),
                TicketId = ticketId,
                SenderType = SenderAI,
                Content = aiResponse.Content,
                AiModelUsed = aiResponse.ModelUsed,
                TokensUsed = aiResponse.TokensUsed
            };

            await _unitOfWork.SupportMessageRepository.AddAsync(aiMessage);

            // Handle action
            if (aiResponse.SuggestedAction != null)
            {
                var action = new SupportAction
                {
                    Id = Guid.NewGuid(),
                    TicketId = ticketId,
                    ActionType = aiResponse.SuggestedAction.ActionType,
                    ActionData = aiResponse.SuggestedAction.ActionDataJson,
                    IsApproved = false,
                    IsExecuted = false
                };
                await _unitOfWork.SupportActionRepository.AddAsync(action);
            }

            // Handle escalation
            if (aiResponse.ShouldEscalate && !ticket.IsEscalated)
            {
                var ticketToUpdate = await _unitOfWork.SupportTicketRepository.GetAsync(
                    x => x.Id == ticketId, enableTracking: true);
                if (ticketToUpdate != null)
                {
                    ticketToUpdate.IsEscalated = true;
                    ticketToUpdate.StatusId = StatusEscalated;
                    await _unitOfWork.SupportTicketRepository.UpdateAsync(ticketToUpdate);
                }
            }

            await _unitOfWork.CompleteAsync();

            result.SetData(new SupportMessageDto
            {
                Id = aiMessage.Id,
                SenderType = SenderAI,
                Content = aiMessage.Content,
                CreatedDate = aiMessage.CreatedDate
            });
        }
        catch (Exception e)
        {
            result.Fail(e);
        }

        return result;
    }

    public async Task<ServiceObjectResult<SupportTicketDetailDto>> GetTicketDetail(Guid ticketId)
    {
        var result = new ServiceObjectResult<SupportTicketDetailDto>();
        try
        {
            var token = _tokenAccessor.GetToken();
            if (token == null)
            {
                result.Fail("Unauthorized");
                return result;
            }

            var ticket = await _unitOfWork.SupportTicketRepository.GetAsync(
                x => x.Id == ticketId && x.UserId == token.UserId);
            if (ticket == null)
            {
                result.Fail("Ticket not found");
                return result;
            }

            var conn = _context.Database.GetDbConnection();
            if (conn.State != ConnectionState.Open)
                await conn.OpenAsync();

            const string messagesQuery = @"
                SELECT `Id`, `SenderType`, `Content`, `CreatedDate`
                FROM `SupportMessage`
                WHERE `TicketId` = @ticketId AND `DeletedDate` IS NULL
                ORDER BY `CreatedDate` ASC";

            const string actionsQuery = @"
                SELECT `Id`, `ActionType`, `ActionData`, `IsApproved`, `IsExecuted`, `CreatedDate`
                FROM `SupportAction`
                WHERE `TicketId` = @ticketId AND `DeletedDate` IS NULL
                ORDER BY `CreatedDate` ASC";

            var messages = (await conn.QueryAsync<SupportMessageDto>(messagesQuery, new { ticketId })).ToList();
            var actions = (await conn.QueryAsync<SupportActionDto>(actionsQuery, new { ticketId })).ToList();

            result.SetData(new SupportTicketDetailDto
            {
                Id = ticket.Id,
                Subject = ticket.Subject,
                TopicId = ticket.TopicId,
                StatusId = ticket.StatusId,
                IsEscalated = ticket.IsEscalated,
                Rating = ticket.Rating,
                RatingComment = ticket.RatingComment,
                OrderId = ticket.OrderId,
                RestaurantId = ticket.RestaurantId,
                CreatedDate = ticket.CreatedDate,
                ResolvedAt = ticket.ResolvedAt,
                ClosedAt = ticket.ClosedAt,
                Messages = messages,
                Actions = actions
            });
        }
        catch (Exception e)
        {
            result.Fail(e);
        }

        return result;
    }

    public async Task<ServiceCollectionResult<SupportTicketDto>> GetMyTickets(short? statusId = null, int page = 1, int pageSize = 20)
    {
        var result = new ServiceCollectionResult<SupportTicketDto>();
        try
        {
            var token = _tokenAccessor.GetToken();
            if (token == null)
            {
                result.Fail("Unauthorized");
                return result;
            }

            pageSize = Math.Min(pageSize, 50);
            var offset = (page - 1) * pageSize;

            var query = $@"
                SELECT t.`Id`, t.`Subject`, t.`TopicId`, t.`StatusId`, t.`IsEscalated`,
                       t.`Rating`, t.`CreatedDate`, t.`ResolvedAt`,
                       (SELECT COUNT(*) FROM `SupportMessage` WHERE `TicketId` = t.`Id` AND `DeletedDate` IS NULL) AS MessageCount
                FROM `SupportTicket` t
                WHERE t.`UserId` = @userId AND t.`DeletedDate` IS NULL
                {(statusId.HasValue ? "AND t.`StatusId` = @statusId" : "")}
                ORDER BY t.`CreatedDate` DESC
                LIMIT @pageSize OFFSET @offset";

            var countQuery = $@"SELECT COUNT(*) FROM `SupportTicket`
                WHERE `UserId` = @userId AND `DeletedDate` IS NULL
                {(statusId.HasValue ? "AND `StatusId` = @statusId" : "")}";

            var conn = _context.Database.GetDbConnection();
            if (conn.State != ConnectionState.Open)
                await conn.OpenAsync();

            var totalCount = await conn.ExecuteScalarAsync<int>(countQuery, new { userId = token.UserId, statusId });
            var tickets = (await conn.QueryAsync<SupportTicketDto>(query, new { userId = token.UserId, statusId, pageSize, offset })).ToList();

            result.SetData(totalCount, tickets);
        }
        catch (Exception e)
        {
            result.Fail(e);
        }

        return result;
    }

    public async Task<ServiceObjectResult<bool>> CloseTicket(Guid ticketId)
    {
        var result = new ServiceObjectResult<bool>();
        try
        {
            var token = _tokenAccessor.GetToken();
            if (token == null)
            {
                result.Fail("Unauthorized");
                return result;
            }

            var ticket = await _unitOfWork.SupportTicketRepository.GetAsync(
                x => x.Id == ticketId && x.UserId == token.UserId, enableTracking: true);
            if (ticket == null)
            {
                result.Fail("Ticket not found");
                return result;
            }

            if (ticket.StatusId == StatusClosed)
            {
                result.Fail("Ticket is already closed");
                return result;
            }

            ticket.StatusId = StatusClosed;
            ticket.ClosedAt = DateTime.UtcNow;
            if (ticket.ResolvedAt == null)
                ticket.ResolvedAt = DateTime.UtcNow;

            await _unitOfWork.SupportTicketRepository.UpdateAsync(ticket);
            await _unitOfWork.CompleteAsync();

            result.SetData(true);
        }
        catch (Exception e)
        {
            result.Fail(e);
        }

        return result;
    }

    public async Task<ServiceObjectResult<bool>> RateTicket(Guid ticketId, RateTicketRequestDto requestDto)
    {
        var result = new ServiceObjectResult<bool>();
        try
        {
            var token = _tokenAccessor.GetToken();
            if (token == null)
            {
                result.Fail("Unauthorized");
                return result;
            }

            if (requestDto.Rating < 1 || requestDto.Rating > 5)
            {
                result.Fail("Rating must be between 1 and 5");
                return result;
            }

            var ticket = await _unitOfWork.SupportTicketRepository.GetAsync(
                x => x.Id == ticketId && x.UserId == token.UserId, enableTracking: true);
            if (ticket == null)
            {
                result.Fail("Ticket not found");
                return result;
            }

            ticket.Rating = requestDto.Rating;
            ticket.RatingComment = requestDto.Comment;

            await _unitOfWork.SupportTicketRepository.UpdateAsync(ticket);
            await _unitOfWork.CompleteAsync();

            result.SetData(true);
        }
        catch (Exception e)
        {
            result.Fail(e);
        }

        return result;
    }

    public async Task<ServiceCollectionResult<SupportTicketDto>> GetEscalatedTickets(int page = 1, int pageSize = 20)
    {
        var result = new ServiceCollectionResult<SupportTicketDto>();
        try
        {
            pageSize = Math.Min(pageSize, 50);
            var offset = (page - 1) * pageSize;

            const string countQuery = @"
                SELECT COUNT(*) FROM `SupportTicket`
                WHERE `IsEscalated` = 1 AND `StatusId` != @closedStatus AND `DeletedDate` IS NULL";

            const string query = @"
                SELECT t.`Id`, t.`Subject`, t.`TopicId`, t.`StatusId`, t.`IsEscalated`,
                       t.`Rating`, t.`CreatedDate`, t.`ResolvedAt`,
                       (SELECT COUNT(*) FROM `SupportMessage` WHERE `TicketId` = t.`Id` AND `DeletedDate` IS NULL) AS MessageCount
                FROM `SupportTicket` t
                WHERE t.`IsEscalated` = 1 AND t.`StatusId` != @closedStatus AND t.`DeletedDate` IS NULL
                ORDER BY t.`CreatedDate` DESC
                LIMIT @pageSize OFFSET @offset";

            var conn = _context.Database.GetDbConnection();
            if (conn.State != ConnectionState.Open)
                await conn.OpenAsync();

            var totalCount = await conn.ExecuteScalarAsync<int>(countQuery, new { closedStatus = StatusClosed });
            var tickets = (await conn.QueryAsync<SupportTicketDto>(query, new { closedStatus = StatusClosed, pageSize, offset })).ToList();

            result.SetData(totalCount, tickets);
        }
        catch (Exception e)
        {
            result.Fail(e);
        }

        return result;
    }

    public async Task<ServiceObjectResult<bool>> ApproveAction(Guid actionId)
    {
        var result = new ServiceObjectResult<bool>();
        try
        {
            var action = await _unitOfWork.SupportActionRepository.GetAsync(
                x => x.Id == actionId, enableTracking: true);
            if (action == null)
            {
                result.Fail("Action not found");
                return result;
            }

            if (action.IsApproved)
            {
                result.Fail("Action is already approved");
                return result;
            }

            var token = _tokenAccessor.GetToken();
            action.IsApproved = true;
            action.ApprovedByUserId = token?.UserId;

            // TODO: Execute the action (refund, cancel, etc.)
            action.IsExecuted = true;
            action.ExecutedAt = DateTime.UtcNow;

            await _unitOfWork.SupportActionRepository.UpdateAsync(action);
            await _unitOfWork.CompleteAsync();

            result.SetData(true);
        }
        catch (Exception e)
        {
            result.Fail(e);
        }

        return result;
    }

    private async Task<AiSupportContext> BuildAiContext(SupportTicket ticket, List<SupportMessage> messages)
    {
        var context = new AiSupportContext
        {
            Topic = GetTopicName(ticket.TopicId)
        };

        // Map messages to conversation history
        foreach (var msg in messages)
        {
            context.ConversationHistory.Add(new AiConversationMessage
            {
                Role = msg.SenderType == SenderCustomer ? "user" : "assistant",
                Content = msg.Content
            });
        }

        // Load order context if available
        if (ticket.OrderId.HasValue)
        {
            var conn = _context.Database.GetDbConnection();
            if (conn.State != ConnectionState.Open)
                await conn.OpenAsync();

            const string orderQuery = @"
                SELECT o.`Id`, o.`StatusId`, o.`TotalPrice`, o.`CreatedDate`,
                       r.`Name` AS RestaurantName
                FROM `Order` o
                INNER JOIN `Restaurant` r ON r.`Id` = o.`RestaurantId`
                WHERE o.`Id` = @orderId";

            var orderInfo = await conn.QueryFirstOrDefaultAsync<dynamic>(orderQuery, new { orderId = ticket.OrderId.Value });
            if (orderInfo != null)
                context.OrderSummaryJson = JsonSerializer.Serialize(orderInfo);
        }

        // Load restaurant context if available
        if (ticket.RestaurantId.HasValue)
        {
            var conn = _context.Database.GetDbConnection();
            if (conn.State != ConnectionState.Open)
                await conn.OpenAsync();

            const string restaurantQuery = @"
                SELECT `Id`, `Name`, `PhoneNumber`
                FROM `Restaurant`
                WHERE `Id` = @restaurantId";

            var restaurantInfo = await conn.QueryFirstOrDefaultAsync<dynamic>(restaurantQuery, new { restaurantId = ticket.RestaurantId.Value });
            if (restaurantInfo != null)
                context.RestaurantInfoJson = JsonSerializer.Serialize(restaurantInfo);
        }

        return context;
    }

    public async Task<ServiceCollectionResult<SupportTicketDto>> GetAllTickets(short? statusId = null, short? topicId = null, int page = 1, int pageSize = 20)
    {
        var result = new ServiceCollectionResult<SupportTicketDto>();
        try
        {
            pageSize = Math.Min(pageSize, 50);
            var offset = (page - 1) * pageSize;

            var where = "t.`DeletedDate` IS NULL";
            if (statusId.HasValue) where += " AND t.`StatusId` = @statusId";
            if (topicId.HasValue) where += " AND t.`TopicId` = @topicId";

            var countQuery = $"SELECT COUNT(*) FROM `SupportTicket` t WHERE {where}";
            var query = $@"
                SELECT t.`Id`, t.`Subject`, t.`TopicId`, t.`StatusId`, t.`IsEscalated`,
                       t.`Rating`, t.`CreatedDate`, t.`ResolvedAt`,
                       (SELECT COUNT(*) FROM `SupportMessage` WHERE `TicketId` = t.`Id` AND `DeletedDate` IS NULL) AS MessageCount
                FROM `SupportTicket` t
                WHERE {where}
                ORDER BY t.`CreatedDate` DESC
                LIMIT @pageSize OFFSET @offset";

            var conn = _context.Database.GetDbConnection();
            if (conn.State != ConnectionState.Open)
                await conn.OpenAsync();

            var totalCount = await conn.ExecuteScalarAsync<int>(countQuery, new { statusId, topicId });
            var tickets = (await conn.QueryAsync<SupportTicketDto>(query, new { statusId, topicId, pageSize, offset })).ToList();

            result.SetData(totalCount, tickets);
        }
        catch (Exception e)
        {
            result.Fail(e);
        }
        return result;
    }

    public async Task<ServiceObjectResult<SupportTicketDetailDto>> GetTicketDetailAdmin(Guid ticketId)
    {
        var result = new ServiceObjectResult<SupportTicketDetailDto>();
        try
        {
            var ticket = await _unitOfWork.SupportTicketRepository.GetAsync(x => x.Id == ticketId);
            if (ticket == null)
            {
                result.Fail("Ticket not found");
                return result;
            }

            var conn = _context.Database.GetDbConnection();
            if (conn.State != ConnectionState.Open)
                await conn.OpenAsync();

            const string messagesQuery = @"
                SELECT `Id`, `SenderType`, `Content`, `CreatedDate`
                FROM `SupportMessage`
                WHERE `TicketId` = @ticketId AND `DeletedDate` IS NULL
                ORDER BY `CreatedDate` ASC";

            const string actionsQuery = @"
                SELECT `Id`, `ActionType`, `ActionData`, `IsApproved`, `IsExecuted`, `CreatedDate`
                FROM `SupportAction`
                WHERE `TicketId` = @ticketId AND `DeletedDate` IS NULL
                ORDER BY `CreatedDate` ASC";

            var messages = (await conn.QueryAsync<SupportMessageDto>(messagesQuery, new { ticketId })).ToList();
            var actions = (await conn.QueryAsync<SupportActionDto>(actionsQuery, new { ticketId })).ToList();

            result.SetData(new SupportTicketDetailDto
            {
                Id = ticket.Id,
                Subject = ticket.Subject,
                TopicId = ticket.TopicId,
                StatusId = ticket.StatusId,
                IsEscalated = ticket.IsEscalated,
                Rating = ticket.Rating,
                RatingComment = ticket.RatingComment,
                OrderId = ticket.OrderId,
                RestaurantId = ticket.RestaurantId,
                CreatedDate = ticket.CreatedDate,
                ResolvedAt = ticket.ResolvedAt,
                ClosedAt = ticket.ClosedAt,
                Messages = messages,
                Actions = actions
            });
        }
        catch (Exception e)
        {
            result.Fail(e);
        }
        return result;
    }

    public async Task<ServiceObjectResult<SupportStatsDto>> GetSupportStats()
    {
        var result = new ServiceObjectResult<SupportStatsDto>();
        try
        {
            const string query = @"
                SELECT
                    COUNT(*) AS TotalTickets,
                    SUM(CASE WHEN `StatusId` = 1 THEN 1 ELSE 0 END) AS OpenTickets,
                    SUM(CASE WHEN `IsEscalated` = 1 AND `StatusId` NOT IN (4) THEN 1 ELSE 0 END) AS EscalatedTickets,
                    SUM(CASE WHEN `StatusId` = 3 THEN 1 ELSE 0 END) AS ResolvedTickets,
                    SUM(CASE WHEN `StatusId` = 4 THEN 1 ELSE 0 END) AS ClosedTickets,
                    COALESCE(AVG(CASE WHEN `Rating` IS NOT NULL THEN `Rating` END), 0) AS AverageRating,
                    COALESCE(AVG(CASE WHEN `ResolvedAt` IS NOT NULL THEN TIMESTAMPDIFF(HOUR, `CreatedDate`, `ResolvedAt`) END), 0) AS AverageResolutionHours
                FROM `SupportTicket`
                WHERE `DeletedDate` IS NULL";

            var conn = _context.Database.GetDbConnection();
            if (conn.State != ConnectionState.Open)
                await conn.OpenAsync();

            var stats = await conn.QueryFirstOrDefaultAsync<SupportStatsDto>(query);
            result.SetData(stats ?? new SupportStatsDto());
        }
        catch (Exception e)
        {
            result.Fail(e);
        }
        return result;
    }

    public async Task<ServiceObjectResult<bool>> RejectAction(Guid actionId)
    {
        var result = new ServiceObjectResult<bool>();
        try
        {
            var action = await _unitOfWork.SupportActionRepository.GetAsync(
                x => x.Id == actionId, enableTracking: true);
            if (action == null)
            {
                result.Fail("Action not found");
                return result;
            }

            if (action.IsApproved || action.IsExecuted)
            {
                result.Fail("Action is already processed");
                return result;
            }

            await _unitOfWork.SupportActionRepository.DeleteAsync(action);
            await _unitOfWork.CompleteAsync();

            result.SetData(true);
        }
        catch (Exception e)
        {
            result.Fail(e);
        }
        return result;
    }

    private static string GetTopicName(short topicId) => topicId switch
    {
        1 => "Siparis Sorunu",
        2 => "Iptal Talebi",
        3 => "Teslimat Problemi",
        4 => "Genel Soru",
        5 => "Hesap Sorunu",
        6 => "Odeme Sorunu",
        _ => "Genel"
    };
}
