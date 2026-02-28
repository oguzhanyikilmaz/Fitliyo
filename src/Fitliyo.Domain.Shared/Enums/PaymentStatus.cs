namespace Fitliyo.Enums;

/// <summary>
/// Ödeme durumu
/// </summary>
public enum PaymentStatus
{
    Pending = 0,
    Paid = 1,
    Escrow = 2,
    Released = 3,
    Refunded = 4,
    Failed = 5
}
