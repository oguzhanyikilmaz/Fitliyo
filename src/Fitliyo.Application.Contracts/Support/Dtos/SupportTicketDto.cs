using System;
using Fitliyo.Enums;
using Volo.Abp.Application.Dtos;

namespace Fitliyo.Support.Dtos;

public class SupportTicketDto : EntityDto<Guid>
{
    public string Subject { get; set; } = default!;
    public string Message { get; set; } = default!;
    public Guid? UserId { get; set; }
    public Guid? OrderId { get; set; }
    public SupportTicketCategory Category { get; set; }
    public SupportTicketStatus Status { get; set; }
    public TicketPriority Priority { get; set; }
    public string? AdminReply { get; set; }
    public DateTime? AdminReplyDate { get; set; }
    public Guid? AssignedToUserId { get; set; }
    public DateTime CreationTime { get; set; }
}
