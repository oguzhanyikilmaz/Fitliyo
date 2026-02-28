using Fitliyo.Enums;
using Volo.Abp.Application.Dtos;

namespace Fitliyo.Notifications.Dtos;

public class GetNotificationListDto : PagedResultRequestDto
{
    public bool? IsRead { get; set; }
    public NotificationType? NotificationType { get; set; }
}
