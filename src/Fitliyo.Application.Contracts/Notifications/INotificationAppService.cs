using System;
using System.Threading.Tasks;
using Fitliyo.Notifications.Dtos;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace Fitliyo.Notifications;

public interface INotificationAppService : IApplicationService
{
    Task<PagedResultDto<NotificationDto>> GetMyNotificationsAsync(GetNotificationListDto input);

    Task<int> GetUnreadCountAsync();

    Task MarkAsReadAsync(Guid id);

    Task MarkAllAsReadAsync();
}
