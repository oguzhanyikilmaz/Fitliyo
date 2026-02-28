using System;
using System.ComponentModel.DataAnnotations;
using Fitliyo.Enums;
using Fitliyo.Orders;
using Volo.Abp;
using Volo.Abp.Domain.Entities.Auditing;

namespace Fitliyo.Orders;

/// <summary>
/// Sipariş — Öğrencinin bir paketi satın alması
/// </summary>
public class Order : FullAuditedAggregateRoot<Guid>
{
    [Required]
    [StringLength(OrderConsts.MaxOrderNumberLength)]
    public string OrderNumber { get; private set; } = default!;

    /// <summary>
    /// Satın alan öğrenci
    /// </summary>
    [Required]
    public Guid StudentId { get; private set; }

    /// <summary>
    /// Eğitmen profili
    /// </summary>
    [Required]
    public Guid TrainerProfileId { get; private set; }

    /// <summary>
    /// Satın alınan hizmet paketi
    /// </summary>
    [Required]
    public Guid ServicePackageId { get; private set; }

    public OrderStatus Status { get; set; }

    public PaymentStatus PaymentStatus { get; set; }

    /// <summary>
    /// Paket birim fiyatı (sipariş anında sabitlenen)
    /// </summary>
    public decimal UnitPrice { get; set; }

    public int Quantity { get; set; } = 1;

    /// <summary>
    /// Toplam tutar (indirim öncesi)
    /// </summary>
    public decimal TotalAmount { get; set; }

    /// <summary>
    /// İndirim tutarı
    /// </summary>
    public decimal DiscountAmount { get; set; }

    /// <summary>
    /// Net ödenecek tutar
    /// </summary>
    public decimal NetAmount { get; set; }

    /// <summary>
    /// Platform komisyonu
    /// </summary>
    public decimal CommissionAmount { get; set; }

    /// <summary>
    /// Eğitmene aktarılacak tutar
    /// </summary>
    public decimal TrainerPayoutAmount { get; set; }

    [Required]
    [StringLength(OrderConsts.MaxCurrencyLength)]
    public string Currency { get; set; } = OrderConsts.DefaultCurrency;

    /// <summary>
    /// Ödeme sağlayıcı (iyzico, stripe)
    /// </summary>
    [StringLength(OrderConsts.MaxPaymentProviderLength)]
    public string? PaymentProvider { get; set; }

    /// <summary>
    /// Ödeme sağlayıcı işlem ID'si
    /// </summary>
    [StringLength(OrderConsts.MaxPaymentTransactionIdLength)]
    public string? PaymentTransactionId { get; set; }

    public DateTime? PaidAt { get; set; }

    public DateTime? CompletedAt { get; set; }

    public DateTime? CancelledAt { get; set; }

    [StringLength(OrderConsts.MaxCancellationReasonLength)]
    public string? CancellationReason { get; set; }

    [StringLength(OrderConsts.MaxNotesLength)]
    public string? Notes { get; set; }

    protected Order()
    {
    }

    public Order(
        Guid id,
        string orderNumber,
        Guid studentId,
        Guid trainerProfileId,
        Guid servicePackageId,
        decimal unitPrice,
        int quantity)
        : base(id)
    {
        OrderNumber = Check.NotNullOrWhiteSpace(orderNumber, nameof(orderNumber), OrderConsts.MaxOrderNumberLength);
        StudentId = studentId;
        TrainerProfileId = trainerProfileId;
        ServicePackageId = servicePackageId;
        UnitPrice = unitPrice;
        Quantity = quantity;
        TotalAmount = unitPrice * quantity;
        NetAmount = TotalAmount;
        CommissionAmount = NetAmount * OrderConsts.PlatformCommissionRate;
        TrainerPayoutAmount = NetAmount - CommissionAmount;
        Status = OrderStatus.Pending;
        PaymentStatus = PaymentStatus.Pending;
        Currency = OrderConsts.DefaultCurrency;
    }

    public void MarkAsPaid(string paymentProvider, string transactionId)
    {
        PaymentProvider = paymentProvider;
        PaymentTransactionId = transactionId;
        PaymentStatus = PaymentStatus.Escrow;
        Status = OrderStatus.Confirmed;
        PaidAt = DateTime.Now;
    }

    public void Complete()
    {
        Status = OrderStatus.Completed;
        PaymentStatus = PaymentStatus.Released;
        CompletedAt = DateTime.Now;
    }

    public void Cancel(string? reason)
    {
        Status = OrderStatus.Cancelled;
        CancellationReason = reason;
        CancelledAt = DateTime.Now;
    }
}
