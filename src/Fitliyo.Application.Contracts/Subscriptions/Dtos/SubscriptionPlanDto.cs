using System;
using Fitliyo.Enums;
using Volo.Abp.Application.Dtos;

namespace Fitliyo.Subscriptions.Dtos;

public class SubscriptionPlanDto : EntityDto<Guid>
{
    public string Name { get; set; } = default!;
    public string? Description { get; set; }
    public SubscriptionTier Tier { get; set; }
    public SubscriptionPlanType PlanType { get; set; }
    public decimal Price { get; set; }
    public string Currency { get; set; } = "TRY";
    public int MaxPackageCount { get; set; }
    public decimal CommissionRate { get; set; }
    public bool HasFeaturedListing { get; set; }
    public bool HasPrioritySupport { get; set; }
    public bool HasAdvancedAnalytics { get; set; }
    public string? FeaturesJson { get; set; }
    public bool IsActive { get; set; }
    public int SortOrder { get; set; }
}
