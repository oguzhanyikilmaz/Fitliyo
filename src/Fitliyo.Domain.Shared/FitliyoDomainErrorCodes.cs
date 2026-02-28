namespace Fitliyo;

public static class FitliyoDomainErrorCodes
{
    /* Trainer */

    /// <summary>Eğitmen profili bulunamadı</summary>
    public const string TrainerProfileNotFound = "Fitliyo:Trainer:00001";

    /// <summary>Bu slug zaten kullanılmaktadır</summary>
    public const string TrainerSlugAlreadyExists = "Fitliyo:Trainer:00002";

    /// <summary>Kullanıcının zaten bir eğitmen profili var</summary>
    public const string TrainerProfileAlreadyExists = "Fitliyo:Trainer:00003";

    /// <summary>Eğitmen sertifikası bulunamadı</summary>
    public const string TrainerCertificateNotFound = "Fitliyo:Trainer:00004";

    /// <summary>Galeri öğesi bulunamadı</summary>
    public const string TrainerGalleryItemNotFound = "Fitliyo:Trainer:00005";

    /* Package */

    /// <summary>Hizmet paketi bulunamadı</summary>
    public const string PackageNotFound = "Fitliyo:Package:00001";

    /// <summary>Free planda aylık paket limiti aşıldı</summary>
    public const string PackageLimitExceeded = "Fitliyo:Package:00002";

    /// <summary>Paket dolu, yeni öğrenci kabul edilemiyor</summary>
    public const string PackageFullyBooked = "Fitliyo:Package:00003";

    /* Category */

    /// <summary>Kategori bulunamadı</summary>
    public const string CategoryNotFound = "Fitliyo:Category:00001";

    /// <summary>Kategori slug'ı zaten kullanılmaktadır</summary>
    public const string CategorySlugAlreadyExists = "Fitliyo:Category:00002";

    /* Order */

    /// <summary>Sipariş bulunamadı</summary>
    public const string OrderNotFound = "Fitliyo:Order:00001";

    /// <summary>Sipariş iptal edilemez — mevcut duruma uygun değil</summary>
    public const string OrderCannotBeCancelled = "Fitliyo:Order:00002";

    /// <summary>Ödeme işlemi başarısız</summary>
    public const string PaymentFailed = "Fitliyo:Order:00003";

    /// <summary>Bu pakete ait aktif sipariş zaten var</summary>
    public const string ActiveOrderAlreadyExists = "Fitliyo:Order:00004";

    /// <summary>Seans bulunamadı</summary>
    public const string SessionNotFound = "Fitliyo:Order:00005";

    /// <summary>Seans iptal süresi geçmiş</summary>
    public const string SessionCancellationDeadlinePassed = "Fitliyo:Order:00006";

    /// <summary>Kendi paketinizi satın alamazsınız</summary>
    public const string CannotPurchaseOwnPackage = "Fitliyo:Order:00007";

    /* Review */

    /// <summary>Değerlendirme bulunamadı</summary>
    public const string ReviewNotFound = "Fitliyo:Review:00001";

    /// <summary>Bu sipariş için zaten değerlendirme yapılmış</summary>
    public const string ReviewAlreadyExists = "Fitliyo:Review:00002";

    /// <summary>Sipariş tamamlanmadan değerlendirme yapılamaz</summary>
    public const string OrderNotCompletedForReview = "Fitliyo:Review:00003";

    /// <summary>Değerlendirme süresi dolmuş</summary>
    public const string ReviewPeriodExpired = "Fitliyo:Review:00004";

    /* Messaging */

    /// <summary>Konuşma bulunamadı</summary>
    public const string ConversationNotFound = "Fitliyo:Messaging:00001";

    /// <summary>Kendinize mesaj gönderemezsiniz</summary>
    public const string CannotMessageSelf = "Fitliyo:Messaging:00002";

    /* Subscription */

    /// <summary>Abonelik planı bulunamadı</summary>
    public const string SubscriptionPlanNotFound = "Fitliyo:Subscription:00001";

    /// <summary>Aktif abonelik bulunamadı</summary>
    public const string ActiveSubscriptionNotFound = "Fitliyo:Subscription:00002";

    /// <summary>Bu plana zaten abone</summary>
    public const string AlreadySubscribed = "Fitliyo:Subscription:00003";

    /* Notification */

    /// <summary>Bildirim bulunamadı</summary>
    public const string NotificationNotFound = "Fitliyo:Notification:00001";

    /* Genel */

    /// <summary>Kayıt bulunamadı</summary>
    public const string EntityNotFound = "Fitliyo:General:00001";

    /// <summary>Bu işlem için yetkiniz bulunmamaktadır</summary>
    public const string UnauthorizedAccess = "Fitliyo:General:00002";
}
