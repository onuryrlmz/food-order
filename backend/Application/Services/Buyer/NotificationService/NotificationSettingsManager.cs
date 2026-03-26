using System.Data;
using Application.Services.Common.TokenService;
using Dapper;
using Domain.Dto.Buyer.Notification;
using Domain.Entities.Buyer;
using Domain.Service;
using Microsoft.EntityFrameworkCore;
using Persistence.Contexts;
using Persistence.IRepositories;

namespace Application.Services.Buyer.NotificationService;

public class NotificationSettingsManager : INotificationSettingsService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ITokenAccessor _tokenAccessor;
    private readonly BaseDbContext _context;

    public NotificationSettingsManager(IUnitOfWork unitOfWork, ITokenAccessor tokenAccessor, BaseDbContext context)
    {
        _unitOfWork = unitOfWork;
        _tokenAccessor = tokenAccessor;
        _context = context;
    }

    public async Task<ServiceCollectionResult<NotificationDto>> GetNotifications(int page = 1, int pageSize = 20)
    {
        var result = new ServiceCollectionResult<NotificationDto>();
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

            const string countQuery = @"
                SELECT COUNT(*) FROM `Notification`
                WHERE `UserId` = @userId AND `DeletedDate` IS NULL";

            const string query = @"
                SELECT `Id`, `TypeId`, `Title`, `Message`, `Data`, `IsRead`, `CreatedDate`
                FROM `Notification`
                WHERE `UserId` = @userId AND `DeletedDate` IS NULL
                ORDER BY `CreatedDate` DESC
                LIMIT @pageSize OFFSET @offset";

            var conn = _context.Database.GetDbConnection();
            if (conn.State != ConnectionState.Open)
                await conn.OpenAsync();

            var totalCount = await conn.ExecuteScalarAsync<int>(countQuery, new { userId = token.UserId });
            var notifications = (await conn.QueryAsync<NotificationDto>(query, new { userId = token.UserId, pageSize, offset })).ToList();

            result.SetData(totalCount, notifications);
        }
        catch (Exception e)
        {
            result.Fail(e);
        }

        return result;
    }

    public async Task<ServiceObjectResult<UnreadCountDto>> GetUnreadCount()
    {
        var result = new ServiceObjectResult<UnreadCountDto>();
        try
        {
            var token = _tokenAccessor.GetToken();
            if (token == null)
            {
                result.Fail("Unauthorized");
                return result;
            }

            const string query = @"
                SELECT COUNT(*) FROM `Notification`
                WHERE `UserId` = @userId AND `IsRead` = 0 AND `DeletedDate` IS NULL";

            var conn = _context.Database.GetDbConnection();
            if (conn.State != ConnectionState.Open)
                await conn.OpenAsync();

            var count = await conn.ExecuteScalarAsync<int>(query, new { userId = token.UserId });

            result.SetData(new UnreadCountDto { Count = count });
        }
        catch (Exception e)
        {
            result.Fail(e);
        }

        return result;
    }

    public async Task<ServiceObjectResult<bool>> MarkAsRead(Guid notificationId)
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

            var notification = await _unitOfWork.NotificationRepository.GetAsync(
                x => x.Id == notificationId && x.UserId == token.UserId, enableTracking: true);
            if (notification == null)
            {
                result.Fail("Notification not found");
                return result;
            }

            notification.IsRead = true;
            notification.ReadAt = DateTime.UtcNow;
            await _unitOfWork.NotificationRepository.UpdateAsync(notification);
            await _unitOfWork.CompleteAsync();

            result.SetData(true);
        }
        catch (Exception e)
        {
            result.Fail(e);
        }

        return result;
    }

    public async Task<ServiceObjectResult<bool>> MarkAllAsRead()
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

            const string query = @"
                UPDATE `Notification`
                SET `IsRead` = 1, `ReadAt` = @now, `UpdatedDate` = @now
                WHERE `UserId` = @userId AND `IsRead` = 0 AND `DeletedDate` IS NULL";

            var conn = _context.Database.GetDbConnection();
            if (conn.State != ConnectionState.Open)
                await conn.OpenAsync();

            await conn.ExecuteAsync(query, new { userId = token.UserId, now = DateTime.UtcNow });

            result.SetData(true);
        }
        catch (Exception e)
        {
            result.Fail(e);
        }

        return result;
    }

    public async Task<ServiceCollectionResult<NotificationPreferenceDto>> GetPreferences()
    {
        var result = new ServiceCollectionResult<NotificationPreferenceDto>();
        try
        {
            var token = _tokenAccessor.GetToken();
            if (token == null)
            {
                result.Fail("Unauthorized");
                return result;
            }

            const string query = @"
                SELECT `NotificationTypeId`, `IsEnabled`
                FROM `NotificationPreference`
                WHERE `UserId` = @userId AND `DeletedDate` IS NULL";

            var conn = _context.Database.GetDbConnection();
            if (conn.State != ConnectionState.Open)
                await conn.OpenAsync();

            var prefs = (await conn.QueryAsync<NotificationPreferenceDto>(query, new { userId = token.UserId })).ToList();

            // Add defaults for missing types (1=OrderStatus, 2=Promotions, 3=ReviewResponses, 4=DeliveryUpdates)
            var existingTypes = prefs.Select(p => p.NotificationTypeId).ToHashSet();
            for (short typeId = 1; typeId <= 4; typeId++)
            {
                if (!existingTypes.Contains(typeId))
                    prefs.Add(new NotificationPreferenceDto { NotificationTypeId = typeId, IsEnabled = true });
            }

            result.SetData(prefs.OrderBy(p => p.NotificationTypeId).ToList());
        }
        catch (Exception e)
        {
            result.Fail(e);
        }

        return result;
    }

    public async Task<ServiceObjectResult<bool>> UpdatePreference(UpdateNotificationPreferenceRequestDto requestDto)
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

            if (requestDto.NotificationTypeId < 1 || requestDto.NotificationTypeId > 4)
            {
                result.Fail("Invalid notification type");
                return result;
            }

            var pref = await _unitOfWork.NotificationPreferenceRepository.GetAsync(
                x => x.UserId == token.UserId && x.NotificationTypeId == requestDto.NotificationTypeId,
                enableTracking: true);

            if (pref != null)
            {
                pref.IsEnabled = requestDto.IsEnabled;
                await _unitOfWork.NotificationPreferenceRepository.UpdateAsync(pref);
            }
            else
            {
                var newPref = new NotificationPreference
                {
                    Id = Guid.NewGuid(),
                    UserId = token.UserId,
                    NotificationTypeId = requestDto.NotificationTypeId,
                    IsEnabled = requestDto.IsEnabled
                };
                await _unitOfWork.NotificationPreferenceRepository.AddAsync(newPref);
            }

            await _unitOfWork.CompleteAsync();
            result.SetData(true);
        }
        catch (Exception e)
        {
            result.Fail(e);
        }

        return result;
    }

    public async Task<ServiceObjectResult<bool>> CreateNotification(Guid userId, short typeId, string title, string message, string? data = null, Guid? relatedOrderId = null)
    {
        var result = new ServiceObjectResult<bool>();
        try
        {
            // Check user preference
            var pref = await _unitOfWork.NotificationPreferenceRepository.GetAsync(
                x => x.UserId == userId && x.NotificationTypeId == typeId);

            // Default is enabled if no preference exists
            if (pref != null && !pref.IsEnabled)
            {
                result.SetData(true);
                return result;
            }

            var notification = new Notification
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                TypeId = typeId,
                Title = title,
                Message = message,
                Data = data,
                IsRead = false,
                RelatedOrderId = relatedOrderId
            };

            await _unitOfWork.NotificationRepository.AddAsync(notification);
            await _unitOfWork.CompleteAsync();

            result.SetData(true);
        }
        catch (Exception e)
        {
            result.Fail(e);
        }

        return result;
    }
}
