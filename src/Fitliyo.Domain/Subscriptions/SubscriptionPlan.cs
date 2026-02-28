using System;
using System.ComponentModel.DataAnnotations;
using Fitliyo.Enums;
using Fitliyo.Subscriptions;
using Volo.Abp;
using Volo.Abp.Domain.Entities.Auditing;

namespace Fitliyo.Subscriptions;

/// <summary>
/// Abonelik planı tanımı (Admin tarafından yönetilir)
/// </summary>
public class SubscriptionPlan : FullAuditedAggregateRoot<Guid>
{
    [Required]
    [StringLength(SubscriptionConsts.MaxPlanNameLength)]
    public string Name { get; set; } = default!;

    [StringLength(SubscriptionConsts.MaxPlanDescriptionLength)]
    public string? Description { get; set; }

    /// <summary>
    /// Abonelik seviyesi
    /// </summary>
    public SubscriptionTier Tier { get; set; }

    public SubscriptionPlanType PlanType { get; set; }

    /// <summary>
    /// Aylık fiyat
    /// </summary>
    public decimal Price { get; set; }

    [Required]
    [StringLength(SubscriptionConsts.MaxCurrencyLength)]
    public string Currency { get; set; } = SubscriptionConsts.DefaultCurrency;

    /// <summary>
    /// Maksimum paket sayısı (-1 = sınırsız)
    /// </summary>
    public int MaxPackageCount { get; set; }

    /// <summary>
    /// Komisyon oranı (0.08 = %8)
    /// </summary>
    public decimal CommissionRate { get; set; }

    /// <summary>
    /// Öne çıkarılma hakkı
    /// </summary>
    public bool HasFeaturedListing { get; set; }

    /// <summary>
    /// Öncelikli destek
    /// </summary>
    public bool HasPrioritySupport { get; set; }

    /// <summary>
    /// Detaylı analitik erişimi
    /// </summary>
    public bool HasAdvancedAnalytics { get; set; }

    /// <summary>
    /// Plan özellikleri (JSON)
    /// </summary>
    [StringLength(SubscriptionConsts.MaxFeaturesJsonLength)]
    public string? FeaturesJson { get; set; }

    public bool IsActive { get; set; }

    public int SortOrder { get; set; }

    protected SubscriptionPlan()
    {
    }

    public SubscriptionPlan(Guid id, string name, SubscriptionTier tier, decimal price, decimal commissionRate)
        : base(id)
    {
        Name = Check.NotNullOrWhiteSpace(name, nameof(name), SubscriptionConsts.MaxPlanNameLength);
        Tier = tier;
        Price = price;
        CommissionRate = commissionRate;
        Currency = SubscriptionConsts.DefaultCurrency;
        IsActive = true;
    }
}
