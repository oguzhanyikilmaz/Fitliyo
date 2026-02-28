using System;
using System.ComponentModel.DataAnnotations;
using Fitliyo.Enums;
using Fitliyo.Trainers;
using Volo.Abp;
using Volo.Abp.Domain.Entities.Auditing;

namespace Fitliyo.Trainers;

/// <summary>
/// Eğitmen profili — Marketplace'te listelenen eğitmen bilgileri
/// </summary>
public class TrainerProfile : FullAuditedAggregateRoot<Guid>
{
    /// <summary>
    /// Profili oluşturan kullanıcı (1-1 ilişki)
    /// </summary>
    [Required]
    public Guid UserId { get; private set; }

    /// <summary>
    /// SEO-friendly URL slug (benzersiz)
    /// </summary>
    [Required]
    [StringLength(TrainerConsts.MaxSlugLength)]
    public string Slug { get; private set; } = default!;

    /// <summary>
    /// Eğitmen biyografisi
    /// </summary>
    [StringLength(TrainerConsts.MaxBioLength)]
    public string? Bio { get; set; }

    /// <summary>
    /// Deneyim yılı
    /// </summary>
    public int ExperienceYears { get; set; }

    /// <summary>
    /// Eğitmen uzmanlık alanı
    /// </summary>
    public TrainerType TrainerType { get; set; }

    /// <summary>
    /// Uzmanlık etiketleri (JSON)
    /// </summary>
    public string? SpecialtyTags { get; set; }

    [StringLength(TrainerConsts.MaxCityLength)]
    public string? City { get; set; }

    [StringLength(TrainerConsts.MaxDistrictLength)]
    public string? District { get; set; }

    /// <summary>
    /// Online ders verebilir mi
    /// </summary>
    public bool IsOnlineAvailable { get; set; }

    /// <summary>
    /// Yüz yüze ders verebilir mi
    /// </summary>
    public bool IsOnSiteAvailable { get; set; }

    /// <summary>
    /// Ortalama puan (denormalize — Review modülü tarafından güncellenir)
    /// </summary>
    public decimal AverageRating { get; set; }

    /// <summary>
    /// Toplam değerlendirme sayısı (denormalize)
    /// </summary>
    public int TotalReviewCount { get; set; }

    /// <summary>
    /// Toplam öğrenci sayısı (denormalize)
    /// </summary>
    public int TotalStudentCount { get; set; }

    /// <summary>
    /// Platform tarafından doğrulanmış mı
    /// </summary>
    public bool IsVerified { get; set; }

    [StringLength(TrainerConsts.MaxVerificationBadgeLength)]
    public string? VerificationBadge { get; set; }

    /// <summary>
    /// Abonelik seviyesi
    /// </summary>
    public SubscriptionTier SubscriptionTier { get; set; }

    /// <summary>
    /// Abonelik bitiş tarihi
    /// </summary>
    public DateTime? SubscriptionExpiry { get; set; }

    /// <summary>
    /// Profil tamamlanma yüzdesi (0-100)
    /// </summary>
    public int ProfileCompletionPct { get; set; }

    [StringLength(TrainerConsts.MaxInstagramUrlLength)]
    public string? InstagramUrl { get; set; }

    [StringLength(TrainerConsts.MaxYoutubeUrlLength)]
    public string? YoutubeUrl { get; set; }

    [StringLength(TrainerConsts.MaxWebsiteUrlLength)]
    public string? WebsiteUrl { get; set; }

    /// <summary>
    /// Banka hesap bilgisi (AES-256 şifreli JSON)
    /// </summary>
    public string? BankAccountInfo { get; set; }

    /// <summary>
    /// Profil aktif mi
    /// </summary>
    public bool IsActive { get; set; }

    protected TrainerProfile()
    {
    }

    public TrainerProfile(Guid id, Guid userId, string slug, TrainerType trainerType)
        : base(id)
    {
        UserId = userId;
        SetSlug(slug);
        TrainerType = trainerType;
        IsActive = true;
        SubscriptionTier = SubscriptionTier.Free;
    }

    public void SetSlug(string slug)
    {
        Slug = Check.NotNullOrWhiteSpace(slug, nameof(slug), TrainerConsts.MaxSlugLength);
    }
}
