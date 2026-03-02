namespace Fitliyo.Enums;

/// <summary>
/// Ödeme kaydı durumu (Payment entity — Order.PaymentStatus ile karışmaması için ayrı enum)
/// </summary>
public enum PaymentRecordStatus
{
    Pending = 0,
    Completed = 1,
    Failed = 2,
    Refunded = 3,
    PartialRefund = 4
}
