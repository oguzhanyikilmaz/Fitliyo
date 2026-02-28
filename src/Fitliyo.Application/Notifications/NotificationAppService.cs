using System;
using System.Linq;
using System.Threading.Tasks;
using Fitliyo.Notifications.Dtos;
using Microsoft.AspNetCore.Authorization;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Users;

namespace Fitliyo.Notifications;

[Authorize]
public class NotificationAppService : FitliyoAppService, INotificationAppService
{
    private readonly IRepository<Notification, Guid> _notificationRepository;

    public NotificationAppService(
        IRepository<Notification, Guid> notificationRepository)
    {
        _notificationRepository = notificationRepository;
    }

    [Authorize]
    public async Task<PagedResultDto<NotificationDto>> GetMyNotificationsAsync(GetNotificationListDto input)
    {
        var userId = CurrentUser.GetId();
        var queryable = await _notificationRepository.GetQueryableAsync();

        queryable = queryable.Where(x => x.UserId == userId);

        if (input.IsRead.HasValue)
            queryable = queryable.Where(x => x.IsRead == input.IsRead.Value);

        if (input.NotificationType.HasValue)
            queryable = queryable.Where(x => x.NotificationType == input.NotificationType.Value);

        var totalCount = await AsyncExecuter.CountAsync(queryable);

        queryable = queryable.OrderByDescending(x => x.CreationTime);
        queryable = queryable.PageBy(input);

        var entities = await AsyncExecuter.ToListAsync(queryable);

        return new PagedResultDto<NotificationDto>(totalCount, entities.Select(x => ObjectMapper.Map<Notification, NotificationDto>(x)).ToList());
    }

    [Authorize]
    public async Task<int> GetUnreadCountAsync()
    {
        var userId = CurrentUser.GetId();
        var queryable = await _notificationRepository.GetQueryableAsync();
        return await AsyncExecuter.CountAsync(queryable.Where(x => x.UserId == userId && !x.IsRead));
    }

    [Authorize]
    public async Task MarkAsReadAsync(Guid id)
    {
        var notification = await _notificationRepository.GetAsync(id);
        var userId = CurrentUser.GetId();

        if (notification.UserId != userId) return;

        notification.MarkAsRead();
        await _notificationRepository.UpdateAsync(notification);
    }

    [Authorize]
    public async Task MarkAllAsReadAsync()
    {
        var userId = CurrentUser.GetId();
        var unread = await _notificationRepository.GetListAsync(x => x.UserId == userId && !x.IsRead);

        foreach (var n in unread)
        {
            n.MarkAsRead();
        }

        if (unread.Count > 0)
        {
            await _notificationRepository.UpdateManyAsync(unread);
        }
    }
}
