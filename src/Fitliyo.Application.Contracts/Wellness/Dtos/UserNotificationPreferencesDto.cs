using System;
using Volo.Abp.Application.Dtos;

namespace Fitliyo.Wellness.Dtos;

public class UserNotificationPreferencesDto : EntityDto<Guid>
{
    public Guid UserId { get; set; }
    public bool EmailOrdersAndSessions { get; set; }
    public bool EmailMarketing { get; set; }
    public bool PushChat { get; set; }
    public bool PushOrderSession { get; set; }
    public bool PushWellnessReminders { get; set; }
    public bool InAppAll { get; set; }
}
