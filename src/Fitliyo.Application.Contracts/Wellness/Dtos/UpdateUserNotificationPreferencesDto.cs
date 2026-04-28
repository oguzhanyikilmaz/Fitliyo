namespace Fitliyo.Wellness.Dtos;

public class UpdateUserNotificationPreferencesDto
{
    public bool EmailOrdersAndSessions { get; set; } = true;
    public bool EmailMarketing { get; set; }
    public bool PushChat { get; set; } = true;
    public bool PushOrderSession { get; set; } = true;
    public bool PushWellnessReminders { get; set; } = true;
    public bool InAppAll { get; set; } = true;
}
