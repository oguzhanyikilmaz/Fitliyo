using System;
using Fitliyo.Enums;
using Volo.Abp.Application.Dtos;

namespace Fitliyo.Payments.Dtos;

public class PaymentDto : EntityDto<Guid>
{
    public Guid OrderId { get; set; }
    public PaymentProviderEnum PaymentProvider { get; set; }
    public string ProviderPaymentId { get; set; } = default!;
    public decimal Amount { get; set; }
    public string Currency { get; set; } = "TRY";
    public PaymentRecordStatus Status { get; set; }
    public PaymentMethodEnum PaymentMethod { get; set; }
    public string? CardLastFour { get; set; }
    public string? ReceiptUrl { get; set; }
    public decimal? RefundAmount { get; set; }
    public DateTime? RefundedAt { get; set; }
    public DateTime CreationTime { get; set; }
}
