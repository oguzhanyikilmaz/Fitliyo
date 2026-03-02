using System;
using Fitliyo.Enums;
using Volo.Abp.Application.Dtos;

namespace Fitliyo.Orders.Dtos;

public class OrderDto : EntityDto<Guid>
{
    public string OrderNumber { get; set; } = default!;
    public Guid StudentId { get; set; }
    public Guid TrainerProfileId { get; set; }
    public Guid ServicePackageId { get; set; }
    public OrderStatus Status { get; set; }
    public PaymentStatus PaymentStatus { get; set; }
    public decimal UnitPrice { get; set; }
    public int Quantity { get; set; }
    public decimal TotalAmount { get; set; }
    public decimal DiscountAmount { get; set; }
    public decimal NetAmount { get; set; }
    public decimal CommissionAmount { get; set; }
    public decimal TrainerPayoutAmount { get; set; }
    public string Currency { get; set; } = "TRY";
    public string? PaymentProvider { get; set; }
    public DateTime? PaidAt { get; set; }
    public DateTime? CompletedAt { get; set; }
    public DateTime? CancelledAt { get; set; }
    public string? CancellationReason { get; set; }
    public DateTime CreationTime { get; set; }

    public string? TrainerFullName { get; set; }
    public string? PackageTitle { get; set; }

    /// <summary>
    /// Paketteki seans sayısı (null veya 0 ise seans yok — öğrenci programı kendi uygular)
    /// </summary>
    public int? PackageSessionCount { get; set; }

    /// <summary>
    /// Paket süresi (gün) — program paketlerinde kullanılır (örn. 90 = 3 ay)
    /// </summary>
    public int? PackageDurationDays { get; set; }

    /// <summary>
    /// Öğrencinin eğitmene ilettiği form verisi (JSON: kan değerleri, hedefler vb.)
    /// </summary>
    public string? StudentFormData { get; set; }

    public DateTime? StudentFormSubmittedAt { get; set; }

    /// <summary>
    /// Eğitmenin program notları (öğrenciye gösterilir)
    /// </summary>
    public string? TrainerProgramNotes { get; set; }

    public DateTime? ProgramDeliveredAt { get; set; }

    /// <summary>
    /// Program dosyası/link (PDF vb.)
    /// </summary>
    public string? ProgramAttachmentUrl { get; set; }
}
