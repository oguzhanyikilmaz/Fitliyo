namespace Fitliyo.Enums;

/// <summary>
/// Bildirim tipi
/// </summary>
public enum NotificationType
{
    System = 0,
    OrderCreated = 1,
    OrderConfirmed = 2,
    OrderCancelled = 3,
    OrderCompleted = 4,
    PaymentReceived = 5,
    PayoutReleased = 6,
    SessionReminder = 7,
    SessionCancelled = 8,
    NewReview = 9,
    ReviewReply = 10,
    NewMessage = 11,
    SubscriptionExpiring = 12,
    SubscriptionExpired = 13,
    ProfileVerified = 14,
    PromotionAlert = 15
}
