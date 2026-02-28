using System;
using System.ComponentModel.DataAnnotations;
using Fitliyo.Messaging;
using Volo.Abp;
using Volo.Abp.Domain.Entities.Auditing;

namespace Fitliyo.Messaging;

/// <summary>
/// Mesaj — Konuşma içindeki tek bir mesaj
/// </summary>
public class Message : FullAuditedEntity<Guid>
{
    [Required]
    public Guid ConversationId { get; private set; }

    /// <summary>
    /// Gönderen kullanıcı
    /// </summary>
    [Required]
    public Guid SenderId { get; private set; }

    [Required]
    [StringLength(MessageConsts.MaxContentLength)]
    public string Content { get; set; } = default!;

    [StringLength(MessageConsts.MaxAttachmentUrlLength)]
    public string? AttachmentUrl { get; set; }

    /// <summary>
    /// Okundu mu
    /// </summary>
    public bool IsRead { get; set; }

    public DateTime? ReadAt { get; set; }

    protected Message()
    {
    }

    public Message(Guid id, Guid conversationId, Guid senderId, string content)
        : base(id)
    {
        ConversationId = conversationId;
        SenderId = senderId;
        Content = Check.NotNullOrWhiteSpace(content, nameof(content), MessageConsts.MaxContentLength);
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
