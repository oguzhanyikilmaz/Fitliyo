using System;
using Volo.Abp.Application.Dtos;

namespace Fitliyo.Messaging.Dtos;

public class MessageDto : EntityDto<Guid>
{
    public Guid ConversationId { get; set; }
    public Guid SenderId { get; set; }
    public string Content { get; set; } = default!;
    public string? AttachmentUrl { get; set; }
    public bool IsRead { get; set; }
    public DateTime? ReadAt { get; set; }
    public DateTime CreationTime { get; set; }

    public string? SenderFullName { get; set; }
    public bool IsMine { get; set; }
}
