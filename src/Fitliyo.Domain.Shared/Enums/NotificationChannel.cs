using System;

namespace Fitliyo.Enums;

/// <summary>
/// Bildirim kanalı
/// </summary>
[Flags]
public enum NotificationChannel
{
    InApp = 1,
    Email = 2,
    Push = 4,
    Sms = 8
}
