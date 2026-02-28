using System;
using System.ComponentModel.DataAnnotations;
using Fitliyo.Enums;
using Fitliyo.Subscriptions;
using Volo.Abp.Domain.Entities.Auditing;

namespace Fitliyo.Subscriptions;

/// <summary>
/// Eğitmenin aktif aboneliği
/// </summary>
public class TrainerSubscription : FullAuditedEntity<Guid>
{
    [Required]
    public Guid TrainerProfileId { get; private set; }

    [Required]
    public Guid SubscriptionPlanId { get; private set; }

    public SubscriptionStatus Status { get; set; }

    public DateTime StartDate { get; set; }

    public DateTime EndDate { get; set; }

    /// <summary>
    /// Otomatik yenileme aktif mi
    /// </summary>
    public bool IsAutoRenew { get; set; }

    /// <summary>
    /// Ödeme referans numarası
    /// </summary>
    [StringLength(SubscriptionConsts.MaxPaymentReferenceLength)]
    public string? PaymentReference { get; set; }

    /// <summary>
    /// Ödenen tutar
    /// </summary>
    public decimal PaidAmount { get; set; }

    [Required]
    [StringLength(SubscriptionConsts.MaxCurrencyLength)]
    public string Currency { get; set; } = SubscriptionConsts.DefaultCurrency;

    public DateTime? CancelledAt { get; set; }

    protected TrainerSubscription()
    {
    }

    public TrainerSubscription(
        Guid id,
        Guid trainerProfileId,
        Guid subscriptionPlanId,
        DateTime startDate,
        DateTime endDate,
        decimal paidAmount)
        : base(id)
    {
        TrainerProfileId = trainerProfileId;
        SubscriptionPlanId = subscriptionPlanId;
        StartDate = startDate;
        EndDate = endDate;
        PaidAmount = paidAmount;
        Status = SubscriptionStatus.Active;
        IsAutoRenew = true;
        Currency = SubscriptionConsts.DefaultCurrency;
    }

    public void Cancel()
    {
        Status = SubscriptionStatus.Cancelled;
        IsAutoRenew = false;
        CancelledAt = DateTime.Now;
    }
}
