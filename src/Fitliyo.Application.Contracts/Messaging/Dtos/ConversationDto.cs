using System;
using Volo.Abp.Application.Dtos;

namespace Fitliyo.Messaging.Dtos;

public class ConversationDto : EntityDto<Guid>
{
    public Guid InitiatorId { get; set; }
    public Guid ParticipantId { get; set; }
    public DateTime? LastMessageAt { get; set; }
    public bool IsActive { get; set; }
    public int UnreadCount { get; set; }

    public string? OtherPartyFullName { get; set; }
    public string? OtherPartyProfilePhotoUrl { get; set; }
    public string? LastMessagePreview { get; set; }
}
