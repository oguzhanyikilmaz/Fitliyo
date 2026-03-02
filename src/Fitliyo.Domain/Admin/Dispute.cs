using System;
using System.ComponentModel.DataAnnotations;
using Fitliyo.Admin;
using Fitliyo.Enums;
using Volo.Abp.Domain.Entities.Auditing;

namespace Fitliyo.Admin;

/// <summary>
/// Uyuşmazlık — Sipariş/ödeme ile ilgili ihtilaf kaydı
/// </summary>
public class Dispute : FullAuditedAggregateRoot<Guid>
{
    [Required]
    public Guid OrderId { get; private set; }

    public Guid OpenedByUserId { get; set; }

    public DisputeType DisputeType { get; set; }

    [Required]
    [StringLength(DisputeConsts.MaxDescriptionLength)]
    public string Description { get; set; } = default!;

    public DisputeStatus Status { get; set; }

    [StringLength(DisputeConsts.MaxResolutionNoteLength)]
    public string? ResolutionNote { get; set; }

    public Guid? ResolvedByUserId { get; set; }

    public DateTime? ResolvedAt { get; set; }

    protected Dispute()
    {
    }

    public Dispute(
        Guid id,
        Guid orderId,
        Guid openedByUserId,
        DisputeType disputeType,
        string description)
        : base(id)
    {
        OrderId = orderId;
        OpenedByUserId = openedByUserId;
        DisputeType = disputeType;
        Description = description;
        Status = DisputeStatus.Open;
    }

    public void Resolve(string resolutionNote, Guid resolvedByUserId)
    {
        Status = DisputeStatus.Resolved;
        ResolutionNote = resolutionNote;
        ResolvedByUserId = resolvedByUserId;
        ResolvedAt = DateTime.Now;
    }
}
