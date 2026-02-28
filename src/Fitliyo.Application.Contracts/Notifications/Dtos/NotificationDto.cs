using System;
using Fitliyo.Enums;
using Volo.Abp.Application.Dtos;

namespace Fitliyo.Notifications.Dtos;

public class NotificationDto : EntityDto<Guid>
{
    public Guid UserId { get; set; }
    public NotificationType NotificationType { get; set; }
    public NotificationChannel Channel { get; set; }
    public string Title { get; set; } = default!;
    public string? Body { get; set; }
    public string? ActionUrl { get; set; }
    public bool IsRead { get; set; }
    public DateTime? ReadAt { get; set; }
    public DateTime CreationTime { get; set; }
}
