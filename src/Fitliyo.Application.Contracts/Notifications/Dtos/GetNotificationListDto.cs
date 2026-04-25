using Fitliyo.Enums;
using System;
using Volo.Abp.Application.Dtos;

namespace Fitliyo.Notifications.Dtos;

public class GetNotificationListDto : PagedResultRequestDto
{
    public Guid? UserId { get; set; }
    public bool? IsRead { get; set; }
    public NotificationType? NotificationType { get; set; }
}
