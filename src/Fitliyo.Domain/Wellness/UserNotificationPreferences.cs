using System;
using System.ComponentModel.DataAnnotations;
using Volo.Abp.Domain.Entities.Auditing;

namespace Fitliyo.Wellness;

/// <summary>
/// Kullanıcı bildirim tercihleri (e-posta / push / uygulama içi).
/// </summary>
public class UserNotificationPreferences : FullAuditedAggregateRoot<Guid>
{
    [Required]
    public Guid UserId { get; private set; }

    /// <summary>Sipariş ve seans ile ilgili e-postalar</summary>
    public bool EmailOrdersAndSessions { get; set; } = true;

    /// <summary>Pazarlama / haber bülteni e-postaları</summary>
    public bool EmailMarketing { get; set; }

    /// <summary>Sohbet mesajları push</summary>
    public bool PushChat { get; set; } = true;

    /// <summary>Sipariş / seans push hatırlatmaları</summary>
    public bool PushOrderSession { get; set; } = true;

    /// <summary>Öğün / antrenman / wellness push</summary>
    public bool PushWellnessReminders { get; set; } = true;

    /// <summary>Genel uygulama içi bildirimler (sistem)</summary>
    public bool InAppAll { get; set; } = true;

    protected UserNotificationPreferences()
    {
    }

    public UserNotificationPreferences(Guid id, Guid userId) : base(id)
    {
        UserId = userId;
    }
}
