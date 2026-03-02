using System;
using System.ComponentModel.DataAnnotations;
using Fitliyo.Enums;
using Fitliyo.Payments;
using Volo.Abp.Domain.Entities.Auditing;

namespace Fitliyo.Payments;

/// <summary>
/// Para çekme talebi — Eğitmenin banka hesabına transfer
/// </summary>
public class WithdrawalRequest : FullAuditedAggregateRoot<Guid>
{
    [Required]
    public Guid TrainerWalletId { get; private set; }

    public decimal Amount { get; set; }

    public WithdrawalRequestStatus Status { get; set; }

    [Required]
    [StringLength(WithdrawalConsts.MaxIbanLength)]
    public string Iban { get; set; } = default!;

    [Required]
    [StringLength(WithdrawalConsts.MaxAccountHolderNameLength)]
    public string AccountHolderName { get; set; } = default!;

    [StringLength(WithdrawalConsts.MaxAdminNoteLength)]
    public string? AdminNote { get; set; }

    public DateTime? ProcessedAt { get; set; }

    protected WithdrawalRequest()
    {
    }

    public WithdrawalRequest(
        Guid id,
        Guid trainerWalletId,
        decimal amount,
        string iban,
        string accountHolderName)
        : base(id)
    {
        TrainerWalletId = trainerWalletId;
        Amount = amount;
        Iban = iban;
        AccountHolderName = accountHolderName;
        Status = WithdrawalRequestStatus.Pending;
    }

    public void Approve(string? adminNote = null)
    {
        Status = WithdrawalRequestStatus.Approved;
        AdminNote = adminNote;
    }

    public void Reject(string? adminNote = null)
    {
        Status = WithdrawalRequestStatus.Rejected;
        AdminNote = adminNote;
    }

    public void MarkProcessed()
    {
        Status = WithdrawalRequestStatus.Processed;
        ProcessedAt = DateTime.Now;
    }
}
