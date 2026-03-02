using System;
using System.ComponentModel.DataAnnotations;
using Fitliyo.Enums;
using Fitliyo.Orders;
using Fitliyo.Payments;
using Volo.Abp.Domain.Entities.Auditing;

namespace Fitliyo.Payments;

/// <summary>
/// Ödeme kaydı — Siparişe bağlı tek ödeme veya iade
/// </summary>
public class Payment : FullAuditedAggregateRoot<Guid>
{
    [Required]
    public Guid OrderId { get; private set; }

    public PaymentProviderEnum PaymentProvider { get; set; }

    [Required]
    [StringLength(PaymentConsts.MaxProviderPaymentIdLength)]
    public string ProviderPaymentId { get; set; } = default!;

    public decimal Amount { get; set; }

    [Required]
    [StringLength(PaymentConsts.MaxCurrencyLength)]
    public string Currency { get; set; } = "TRY";

    public PaymentRecordStatus Status { get; set; }

    public PaymentMethodEnum PaymentMethod { get; set; }

    [StringLength(PaymentConsts.MaxCardLastFourLength)]
    public string? CardLastFour { get; set; }

    [StringLength(PaymentConsts.MaxReceiptUrlLength)]
    public string? ReceiptUrl { get; set; }

    public decimal? RefundAmount { get; set; }

    public DateTime? RefundedAt { get; set; }

    /// <summary>
    /// Ham provider yanıtı (log / debug)
    /// </summary>
    [StringLength(PaymentConsts.MaxProviderResponseLength)]
    public string? ProviderResponse { get; set; }

    protected Payment()
    {
    }

    public Payment(
        Guid id,
        Guid orderId,
        PaymentProviderEnum provider,
        string providerPaymentId,
        decimal amount,
        string currency)
        : base(id)
    {
        OrderId = orderId;
        PaymentProvider = provider;
        ProviderPaymentId = providerPaymentId;
        Amount = amount;
        Currency = currency;
        Status = PaymentRecordStatus.Pending;
    }

    public void MarkCompleted()
    {
        Status = PaymentRecordStatus.Completed;
    }

    public void MarkFailed()
    {
        Status = PaymentRecordStatus.Failed;
    }

    public void SetRefund(decimal refundAmount)
    {
        RefundAmount = refundAmount;
        RefundedAt = DateTime.Now;
        Status = refundAmount >= Amount ? PaymentRecordStatus.Refunded : PaymentRecordStatus.PartialRefund;
    }
}
