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
}
