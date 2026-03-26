using Application.Services.Buyer.NotificationService;
using Base.Enums;
using Domain.Dto.Buyer.Notification;
using Domain.Service;
using Microsoft.AspNetCore.Mvc;
using WebAPI.Helpers;

namespace WebAPI.Controllers.Customer;

[Route("v1/customer/notification")]
[ApiController]
public class CustomerNotificationController : BaseController
{
    private readonly INotificationSettingsService _notificationService;

    public CustomerNotificationController(INotificationSettingsService notificationService)
    {
        _notificationService = notificationService;
    }

    [HttpGet]
    [AuthorizeAPIRequest(true, false, UserRoleEnums.User)]
    public async Task<ServiceCollectionResult<NotificationDto>> GetNotifications(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20)
    {
        return await _notificationService.GetNotifications(page, pageSize);
    }

    [HttpGet("unread-count")]
    [AuthorizeAPIRequest(true, false, UserRoleEnums.User)]
    public async Task<ServiceObjectResult<UnreadCountDto>> GetUnreadCount()
    {
        return await _notificationService.GetUnreadCount();
    }

    [HttpPut("{notificationId}/read")]
    [AuthorizeAPIRequest(true, false, UserRoleEnums.User)]
    public async Task<ServiceObjectResult<bool>> MarkAsRead(Guid notificationId)
    {
        return await _notificationService.MarkAsRead(notificationId);
    }

    [HttpPut("read-all")]
    [AuthorizeAPIRequest(true, false, UserRoleEnums.User)]
    public async Task<ServiceObjectResult<bool>> MarkAllAsRead()
    {
        return await _notificationService.MarkAllAsRead();
    }

    [HttpGet("preferences")]
    [AuthorizeAPIRequest(true, false, UserRoleEnums.User)]
    public async Task<ServiceCollectionResult<NotificationPreferenceDto>> GetPreferences()
    {
        return await _notificationService.GetPreferences();
    }

    [HttpPut("preferences")]
    [AuthorizeAPIRequest(true, false, UserRoleEnums.User)]
    public async Task<ServiceObjectResult<bool>> UpdatePreference([FromBody] UpdateNotificationPreferenceRequestDto requestDto)
    {
        return await _notificationService.UpdatePreference(requestDto);
    }
}
