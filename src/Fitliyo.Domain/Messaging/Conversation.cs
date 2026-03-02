using System;
using Volo.Abp.Domain.Entities.Auditing;

namespace Fitliyo.Messaging;

/// <summary>
/// Konuşma — İki kullanıcı arasındaki mesajlaşma kanalı
/// </summary>
public class Conversation : FullAuditedAggregateRoot<Guid>
{
    /// <summary>
    /// Konuşmayı başlatan kullanıcı
    /// </summary>
    public Guid InitiatorId { get; private set; }

    /// <summary>
    /// Karşı taraf
    /// </summary>
    public Guid ParticipantId { get; private set; }

    /// <summary>
    /// Son mesaj zamanı (sıralama için denormalize)
    /// </summary>
    public DateTime? LastMessageAt { get; set; }

    /// <summary>
    /// Konuşma aktif mi
    /// </summary>
    public bool IsActive { get; set; }

    /// <summary>
    /// Bu konuşma belirli bir siparişe mi ait (siparişe özel iletişim)
    /// </summary>
    public Guid? OrderId { get; set; }

    protected Conversation()
    {
    }

    public Conversation(Guid id, Guid initiatorId, Guid participantId, Guid? orderId = null)
        : base(id)
    {
        InitiatorId = initiatorId;
        ParticipantId = participantId;
        OrderId = orderId;
        IsActive = true;
    }
}
