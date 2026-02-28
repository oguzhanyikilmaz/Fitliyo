using System;
using System.Threading.Tasks;
using Fitliyo.Notifications;
using Fitliyo.Notifications.Dtos;
using Microsoft.AspNetCore.Mvc;
using Volo.Abp;
using Volo.Abp.Application.Dtos;

namespace Fitliyo.Controllers;

[RemoteService]
[Area("app")]
[Route("api/app/notifications")]
public class NotificationController : FitliyoController, INotificationAppService
{
    private readonly INotificationAppService _notificationAppService;

    public NotificationController(INotificationAppService notificationAppService)
    {
        _notificationAppService = notificationAppService;
    }

    [HttpGet]
    public Task<PagedResultDto<NotificationDto>> GetMyNotificationsAsync([FromQuery] GetNotificationListDto input)
    {
        return _notificationAppService.GetMyNotificationsAsync(input);
    }

    [HttpGet("unread-count")]
    public Task<int> GetUnreadCountAsync()
    {
        return _notificationAppService.GetUnreadCountAsync();
    }

    [HttpPost("{id}/read")]
    public Task MarkAsReadAsync(Guid id)
    {
        return _notificationAppService.MarkAsReadAsync(id);
    }

    [HttpPost("read-all")]
    public Task MarkAllAsReadAsync()
    {
        return _notificationAppService.MarkAllAsReadAsync();
    }
}
