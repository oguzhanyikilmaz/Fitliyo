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

    protected Conversation()
    {
    }

    public Conversation(Guid id, Guid initiatorId, Guid participantId)
        : base(id)
    {
        InitiatorId = initiatorId;
        ParticipantId = participantId;
        IsActive = true;
    }
}
