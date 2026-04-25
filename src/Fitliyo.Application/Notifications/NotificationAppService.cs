using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Fitliyo.Notifications.Dtos;
using Fitliyo.Permissions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Identity;
using Volo.Abp.Users;

namespace Fitliyo.Notifications;

[Authorize]
public class NotificationAppService : FitliyoAppService, INotificationAppService
{
    private readonly IRepository<Notification, Guid> _notificationRepository;
    private readonly IRepository<IdentityUser, Guid> _identityUserRepository;

    public NotificationAppService(
        IRepository<Notification, Guid> notificationRepository,
        IRepository<IdentityUser, Guid> identityUserRepository)
    {
        _notificationRepository = notificationRepository;
        _identityUserRepository = identityUserRepository;
    }

    [Authorize]
    public async Task<PagedResultDto<NotificationDto>> GetListAsync(GetNotificationListDto input)
    {
        try
        {
            await AuthorizationService.CheckAsync(FitliyoPermissions.Admin.Dashboard);

            var queryable = await _notificationRepository.GetQueryableAsync();

            if (input.UserId.HasValue)
                queryable = queryable.Where(x => x.UserId == input.UserId.Value);

            if (input.IsRead.HasValue)
                queryable = queryable.Where(x => x.IsRead == input.IsRead.Value);

            if (input.NotificationType.HasValue)
                queryable = queryable.Where(x => x.NotificationType == input.NotificationType.Value);

            var totalCount = await AsyncExecuter.CountAsync(queryable);
            queryable = queryable.OrderByDescending(x => x.CreationTime);
            queryable = queryable.PageBy(input);
            var entities = await AsyncExecuter.ToListAsync(queryable);
            var dtos = await MapNotificationsToDtoAsync(entities);
            return new PagedResultDto<NotificationDto>(totalCount, dtos);
        }
        catch (Exception ex) when (IsMissingNotificationsTableError(ex))
        {
            Logger.LogWarning(ex, "AppNotifications tablosu bulunamadı. Geçici olarak boş sonuç dönülüyor.");
            return new PagedResultDto<NotificationDto>(0, []);
        }
    }

    [Authorize]
    public async Task<PagedResultDto<NotificationDto>> GetMyNotificationsAsync(GetNotificationListDto input)
    {
        try
        {
            var userId = (CurrentUser.Id ?? Guid.Empty);
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
            var dtos = await MapNotificationsToDtoAsync(entities);
            return new PagedResultDto<NotificationDto>(totalCount, dtos);
        }
        catch (Exception ex) when (IsMissingNotificationsTableError(ex))
        {
            Logger.LogWarning(ex, "AppNotifications tablosu bulunamadı. Geçici olarak boş sonuç dönülüyor.");
            return new PagedResultDto<NotificationDto>(0, []);
        }
    }

    [Authorize]
    public async Task<int> GetUnreadCountAsync()
    {
        try
        {
            var userId = (CurrentUser.Id ?? Guid.Empty);
            var queryable = await _notificationRepository.GetQueryableAsync();
            return await AsyncExecuter.CountAsync(queryable.Where(x => x.UserId == userId && !x.IsRead));
        }
        catch (Exception ex) when (IsMissingNotificationsTableError(ex))
        {
            Logger.LogWarning(ex, "AppNotifications tablosu bulunamadı. Geçici olarak 0 dönülüyor.");
            return 0;
        }
    }

    [Authorize]
    public async Task MarkAsReadAsync(Guid id)
    {
        try
        {
            var notification = await _notificationRepository.GetAsync(id);
            var userId = (CurrentUser.Id ?? Guid.Empty);

            if (notification.UserId != userId) return;

            notification.MarkAsRead();
            await _notificationRepository.UpdateAsync(notification);
        }
        catch (Exception ex) when (IsMissingNotificationsTableError(ex))
        {
            Logger.LogWarning(ex, "AppNotifications tablosu bulunamadı. MarkAsRead atlandı.");
        }
    }

    [Authorize]
    public async Task MarkAllAsReadAsync()
    {
        try
        {
            var userId = (CurrentUser.Id ?? Guid.Empty);
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
        catch (Exception ex) when (IsMissingNotificationsTableError(ex))
        {
            Logger.LogWarning(ex, "AppNotifications tablosu bulunamadı. MarkAllAsRead atlandı.");
        }
    }

    private static bool IsMissingNotificationsTableError(Exception exception)
    {
        var message = exception.ToString();
        return message.Contains("42P01", StringComparison.OrdinalIgnoreCase) &&
               message.Contains("AppNotifications", StringComparison.OrdinalIgnoreCase);
    }

    private async Task<List<NotificationDto>> MapNotificationsToDtoAsync(IReadOnlyList<Notification> notifications)
    {
        if (notifications.Count == 0)
        {
            return [];
        }

        var userIds = notifications.Select(x => x.UserId).Distinct().ToList();
        var usersQuery = await _identityUserRepository.GetQueryableAsync();
        var users = await AsyncExecuter.ToListAsync(usersQuery.Where(x => userIds.Contains(x.Id)));
        var userNameMap = users.ToDictionary(x => x.Id, x => BuildFullName(x.Name, x.Surname));

        var dtos = notifications.Select(x =>
        {
            var dto = ObjectMapper.Map<Notification, NotificationDto>(x);
            dto.UserFullName = userNameMap.TryGetValue(x.UserId, out var fullName) ? fullName : null;
            return dto;
        }).ToList();

        return dtos;
    }

    private static string? BuildFullName(string? name, string? surname)
    {
        var fullName = $"{name} {surname}".Trim();
        return string.IsNullOrWhiteSpace(fullName) ? null : fullName;
    }
}
