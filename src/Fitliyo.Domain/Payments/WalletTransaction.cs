using System;
using System.ComponentModel.DataAnnotations;
using Fitliyo.Enums;
using Fitliyo.Payments;
using Volo.Abp.Domain.Entities.Auditing;

namespace Fitliyo.Payments;

/// <summary>
/// Cüzdan hareketi — Her bakiye değişikliği için kayıt
/// </summary>
public class WalletTransaction : FullAuditedEntity<Guid>
{
    [Required]
    public Guid TrainerWalletId { get; private set; }

    public WalletTransactionType TransactionType { get; set; }

    public decimal Amount { get; set; }

    [Required]
    [StringLength(WalletConsts.MaxDescriptionLength)]
    public string Description { get; set; } = default!;

    /// <summary>
    /// İlgili Order/Payment/WithdrawalRequest Id
    /// </summary>
    public Guid? ReferenceId { get; set; }

    /// <summary>
    /// İşlem sonrası bakiye (denetim için)
    /// </summary>
    public decimal BalanceAfter { get; set; }

    protected WalletTransaction()
    {
    }

    public WalletTransaction(
        Guid id,
        Guid trainerWalletId,
        WalletTransactionType transactionType,
        decimal amount,
        string description,
        decimal balanceAfter,
        Guid? referenceId = null)
        : base(id)
    {
        TrainerWalletId = trainerWalletId;
        TransactionType = transactionType;
        Amount = amount;
        Description = description;
        BalanceAfter = balanceAfter;
        ReferenceId = referenceId;
    }
}
