using System;
using System.ComponentModel.DataAnnotations;
using Fitliyo.Enums;
using Fitliyo.ServicePackages;
using Volo.Abp;
using Volo.Abp.Domain.Entities.Auditing;

namespace Fitliyo.ServicePackages;

/// <summary>
/// Eğitmenin sunduğu hizmet paketi
/// </summary>
public class ServicePackage : FullAuditedAggregateRoot<Guid>
{
    [Required]
    public Guid TrainerProfileId { get; private set; }

    public PackageType PackageType { get; set; }

    [Required]
    [StringLength(PackageConsts.MaxTitleLength)]
    public string Title { get; set; } = default!;

    [StringLength(PackageConsts.MaxDescriptionLength)]
    public string? Description { get; set; }

    /// <summary>
    /// Liste fiyatı
    /// </summary>
    public decimal Price { get; set; }

    /// <summary>
    /// İndirimli fiyat (null ise indirim yok)
    /// </summary>
    public decimal? DiscountedPrice { get; set; }

    [Required]
    [StringLength(PackageConsts.MaxCurrencyLength)]
    public string Currency { get; set; } = PackageConsts.DefaultCurrency;

    /// <summary>
    /// Paket süresi (gün — eğitim paketleri için)
    /// </summary>
    public int? DurationDays { get; set; }

    /// <summary>
    /// Toplam seans sayısı
    /// </summary>
    public int? SessionCount { get; set; }

    /// <summary>
    /// Seans süresi (dakika)
    /// </summary>
    public int? SessionDurationMinutes { get; set; }

    /// <summary>
    /// Maksimum öğrenci sayısı (tekli ders = 1)
    /// </summary>
    public int MaxStudents { get; set; } = 1;

    public bool IsActive { get; set; }

    public bool IsOnline { get; set; }

    public bool IsOnSite { get; set; }

    /// <summary>
    /// İptal bildirimi saat limiti
    /// </summary>
    public int CancellationHours { get; set; } = 24;

    [StringLength(PackageConsts.MaxCancellationPolicyLength)]
    public string? CancellationPolicy { get; set; }

    /// <summary>
    /// Dahil olanlar (JSON)
    /// </summary>
    public string? WhatIsIncluded { get; set; }

    /// <summary>
    /// Dahil olmayanlar (JSON)
    /// </summary>
    public string? WhatIsNotIncluded { get; set; }

    /// <summary>
    /// Etiketler (JSON)
    /// </summary>
    public string? Tags { get; set; }

    /// <summary>
    /// Toplam satış adedi (denormalize)
    /// </summary>
    public int TotalSalesCount { get; set; }

    /// <summary>
    /// Ortalama puan (denormalize)
    /// </summary>
    public decimal AverageRating { get; set; }

    /// <summary>
    /// Öne çıkarılmış mı
    /// </summary>
    public bool IsFeatured { get; set; }

    protected ServicePackage()
    {
    }

    public ServicePackage(Guid id, Guid trainerProfileId, string title, PackageType packageType, decimal price)
        : base(id)
    {
        TrainerProfileId = trainerProfileId;
        Title = Check.NotNullOrWhiteSpace(title, nameof(title), PackageConsts.MaxTitleLength);
        PackageType = packageType;
        Price = price;
        IsActive = true;
        Currency = PackageConsts.DefaultCurrency;
    }
}
