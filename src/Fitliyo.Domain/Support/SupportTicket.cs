using System;
using System.ComponentModel.DataAnnotations;
using Fitliyo.Enums;
using Fitliyo.Support;
using Volo.Abp.Domain.Entities.Auditing;

namespace Fitliyo.Support;

/// <summary>
/// Destek talebi — Kullanıcı veya eğitmenin yardım talebi
/// </summary>
public class SupportTicket : FullAuditedAggregateRoot<Guid>
{
    [Required]
    [StringLength(SupportTicketConsts.MaxSubjectLength)]
    public string Subject { get; set; } = default!;

    [Required]
    [StringLength(SupportTicketConsts.MaxMessageLength)]
    public string Message { get; set; } = default!;

    public Guid? UserId { get; set; }

    public Guid? OrderId { get; set; }

    public SupportTicketCategory Category { get; set; }

    public SupportTicketStatus Status { get; set; }

    public TicketPriority Priority { get; set; }

    [StringLength(SupportTicketConsts.MaxAdminReplyLength)]
    public string? AdminReply { get; set; }

    public DateTime? AdminReplyDate { get; set; }

    public Guid? AssignedToUserId { get; set; }

    protected SupportTicket()
    {
    }

    public SupportTicket(
        Guid id,
        string subject,
        string message,
        SupportTicketCategory category,
        Guid? userId = null,
        Guid? orderId = null)
        : base(id)
    {
        Subject = subject;
        Message = message;
        Category = category;
        UserId = userId;
        OrderId = orderId;
        Status = SupportTicketStatus.Open;
        Priority = TicketPriority.Medium;
    }

    public void SetAdminReply(string reply)
    {
        AdminReply = reply;
        AdminReplyDate = DateTime.Now;
    }
}
