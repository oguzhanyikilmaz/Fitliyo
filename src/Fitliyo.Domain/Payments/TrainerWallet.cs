using System;
using System.ComponentModel.DataAnnotations;
using Fitliyo.Payments;
using Volo.Abp.Domain.Entities.Auditing;

namespace Fitliyo.Payments;

/// <summary>
/// Eğitmen cüzdanı — Bakiye ve özet bilgiler
/// </summary>
public class TrainerWallet : FullAuditedAggregateRoot<Guid>
{
    [Required]
    public Guid TrainerProfileId { get; private set; }

    /// <summary>
    /// Çekilebilir bakiye
    /// </summary>
    public decimal AvailableBalance { get; set; }

    /// <summary>
    /// Escrow'da bekleyen bakiye (seans tamamlanınca Available'a aktarılır)
    /// </summary>
    public decimal PendingBalance { get; set; }

    /// <summary>
    /// Toplam kazanılan
    /// </summary>
    public decimal TotalEarned { get; set; }

    /// <summary>
    /// Toplam çekilen
    /// </summary>
    public decimal TotalWithdrawn { get; set; }

    public DateTime? LastPayoutAt { get; set; }

    protected TrainerWallet()
    {
    }

    public TrainerWallet(Guid id, Guid trainerProfileId)
        : base(id)
    {
        TrainerProfileId = trainerProfileId;
        AvailableBalance = 0;
        PendingBalance = 0;
        TotalEarned = 0;
        TotalWithdrawn = 0;
    }

    public void AddPending(decimal amount)
    {
        PendingBalance += amount;
        TotalEarned += amount;
    }

    public void MovePendingToAvailable(decimal amount)
    {
        if (amount > PendingBalance)
            throw new InvalidOperationException("Yetersiz bekleyen bakiye.");
        PendingBalance -= amount;
        AvailableBalance += amount;
    }

    public void DebitAvailable(decimal amount)
    {
        if (amount > AvailableBalance)
            throw new InvalidOperationException("Yetersiz bakiye.");
        AvailableBalance -= amount;
        TotalWithdrawn += amount;
        LastPayoutAt = DateTime.Now;
    }

    public void RefundPending(decimal amount)
    {
        if (amount > PendingBalance)
            throw new InvalidOperationException("Yetersiz bekleyen bakiye.");
        PendingBalance -= amount;
    }
}
