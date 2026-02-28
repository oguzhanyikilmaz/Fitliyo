using System;
using System.ComponentModel.DataAnnotations;
using Fitliyo.Enums;
using Fitliyo.Notifications;
using Volo.Abp;
using Volo.Abp.Domain.Entities.Auditing;

namespace Fitliyo.Notifications;

/// <summary>
/// Kullanıcı bildirimi
/// </summary>
public class Notification : CreationAuditedEntity<Guid>
{
    [Required]
    public Guid UserId { get; private set; }

    public NotificationType NotificationType { get; set; }

    public NotificationChannel Channel { get; set; }

    [Required]
    [StringLength(NotificationConsts.MaxTitleLength)]
    public string Title { get; set; } = default!;

    [StringLength(NotificationConsts.MaxBodyLength)]
    public string? Body { get; set; }

    /// <summary>
    /// Tıklanınca yönlendirilecek URL
    /// </summary>
    [StringLength(NotificationConsts.MaxActionUrlLength)]
    public string? ActionUrl { get; set; }

    /// <summary>
    /// Ek veri (JSON)
    /// </summary>
    [StringLength(NotificationConsts.MaxDataJsonLength)]
    public string? DataJson { get; set; }

    public bool IsRead { get; set; }

    public DateTime? ReadAt { get; set; }

    /// <summary>
    /// E-posta gönderildi mi
    /// </summary>
    public bool IsEmailSent { get; set; }

    /// <summary>
    /// Push bildirim gönderildi mi
    /// </summary>
    public bool IsPushSent { get; set; }

    protected Notification()
    {
    }

    public Notification(
        Guid id,
        Guid userId,
        NotificationType notificationType,
        NotificationChannel channel,
        string title)
        : base(id)
    {
        UserId = userId;
        NotificationType = notificationType;
        Channel = channel;
        Title = Check.NotNullOrWhiteSpace(title, nameof(title), NotificationConsts.MaxTitleLength);
    }

    public void MarkAsRead()
    {
        if (!IsRead)
        {
            IsRead = true;
            ReadAt = DateTime.Now;
        }
    }
}
